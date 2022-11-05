#nullable disable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using ChinookSystem.DAL;
using ChinookSystem.Entities;
using ChinookSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
#endregion

namespace ChinookSystem.BLL
{
    // Public as it will be called outside the application
    public class PlaylistTrackServices
    {
        #region Constructor for Context Dependencies Dependency
        private readonly ChinookContext _context;

        internal PlaylistTrackServices(ChinookContext context)
        {
            _context = context;
        }
        #endregion

        #region Queries
        public List<PlaylistTrackInfo> PlaylistTrack_FetchPlaylist(string playlistName, string userName)
        {
            if (string.IsNullOrWhiteSpace(playlistName))
            {
                throw new ArgumentNullException("No playlistName submitted");
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException("No userName submitted");
            }

            IEnumerable<PlaylistTrackInfo> results = _context.PlaylistTracks
                                                        .Where(x => x.Playlist.Name.Equals(playlistName) &&
                                                                    x.Playlist.UserName.Equals(userName))
                                                        .Select(x => new PlaylistTrackInfo
                                                        {
                                                            TrackId = x.TrackId,
                                                            TrackNumber = x.TrackNumber,
                                                            SongName = x.Track.Name,
                                                            Milliseconds = x.Track.Milliseconds,
                                                        })
                                                        .OrderBy(x => x.TrackNumber);

            return results.ToList();
        }
        #endregion

        #region Services
        void PlaylistTrack_AddTrack(string playlistName, string userName, int trackId)
        {
            // locals
            Track trackExists = null;
            Playlist playlistExists = null;
            PlaylistTrack playlistTrackExists = null;
            int trackNumber = 0;

            if (string.IsNullOrWhiteSpace(playlistName))
            {
                throw new ArgumentNullException("No playlistName submitted");
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException("No userName submitted");
            }

            trackExists = _context.Tracks
                            .Where(x => x.TrackId == trackId)
                            .Select(x => x)
                            .FirstOrDefault();

            if (trackExists == null)
            {
                throw new ArgumentException("Selected track no longer on file. Refresh track table.");
            }

            // Playlist names must be unique within a user
            playlistExists = _context.Playlists
                                .Where(x => x.Name.Equals(playlistName) && x.UserName.Equals(userName))
                                .Select(x => x)
                                .FirstOrDefault();

            if (playlistExists == null)
            {
                playlistExists = new Playlist()
                {
                    Name = playlistName,
                    UserName = userName
                };

                // Staging the new playlist record
                _context.Playlists.Add(playlistExists);
                trackNumber = 1;
            }

            else
            {
                // BR: a track may only exist once on a playlist
                playlistTrackExists = _context.PlaylistTracks
                                            .Where(x => x.Playlist.Name.Equals(playlistName) &&
                                                        x.Playlist.UserName.Equals(userName) &&
                                                        x.TrackId == trackId
                                                    )
                                            .Select(x => x)
                                            .FirstOrDefault();

                if (playlistTrackExists == null)
                {
                    // Generate the next track number
                    trackNumber = _context.PlaylistTracks
                                        .Where(x => x.Playlist.Name.Equals(playlistName) &&
                                                        x.Playlist.UserName.Equals(userName)
                                                )
                                        .Count();
                    trackNumber++;
                }

                else
                {
                    var songName = _context.Tracks
                                    .Where(s => s.TrackId == trackId)
                                    .Select(s => s.Name)
                                    .SingleOrDefault();

                    throw new Exception($"Selected track ({songName}) already exists on the playlist.");
                }
            }

            // Processing to stage the new track to the playlist
            playlistTrackExists = new PlaylistTrack();

            // Load the data to the new instance of playlistTrack
            playlistTrackExists.TrackNumber = trackNumber;
            playlistTrackExists.TrackId = trackId;

            // *** IMPORTANT ***
            // Solving the issue of setting the playlistId when a new playlist needs to be generated (the instance of the playlist is only staged at this
            // point). The actual SQL record has NOT yet been created. This means that the entity value for the new playlist DOES NOT exist yet. The value
            // on the playlist instance is 0.

            // Solution is built into EntityFramework software and is based on using the navigational property in Playlists pointing to its "child"
            // Staging a typical Add in the past was to reference the entity and use the entity.Add(xxxx)
            //	 _context.PlaylistTrack.Add(xxxx)	[_context. is context instance in VS]
            // If you use this statement, the playlistId would be zero, causing the transaction to ABORT

            // Instead, do the staging using the syntax of "parent.navigationalproperty.Add(xxxx)"
            // playlistexists will be filled with either
            //	 A) a new staged instance
            // 	 B) a copy of the existing playlist instance

            playlistExists.PlaylistTracks.Add(playlistTrackExists);

            // Staging is now complete
            // Commit the work (transaction)
            // Commiting the work needs a .SaveChanges()
            // A transaction has ONLY ONE .SaveChanges()
            // If the .SaveChanges() fails, then all staged work being handled by the .SaveChanges() is ROLLBACK
            _context.SaveChanges();
        }

        public void PlaylistTrack_RemoveTracks(string playlistName, string userName, List<PlaylistTrackTRX> trackListInfo)
        {
            // local variables
            Playlist playlistExists = null;
            PlaylistTrack playlistTrackExists = null;
            int trackNumber = 1;

            // a container is needed to hold x number of Exception messages
            List<Exception> errorList = new List<Exception>();

            if (string.IsNullOrWhiteSpace(playlistName))
            {
                throw new ArgumentNullException("No playlistName submitted");
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException("No userName submitted");
            }

            var count = trackListInfo.Count();
            if (count == 0)
            {
                throw new ArgumentNullException("No list of tracks were submitted");
            }

            playlistExists = _context.Playlists
                                .Where(x => x.Name.Equals(playlistName) && x.UserName.Equals(userName))
                                .Select(x => x)
                                .FirstOrDefault();

            if (playlistExists == null)
            {
                errorList.Add(new ArgumentException($"Playlist {playlistName} does not exist for this user"));
            }

            else
            {
                // Obtain the tracks to keep
                // SelectedTrack is a boolean field. Keep the false items, remove the true items
                // Create a query to extract the keep tracks from the incoming data
                IEnumerable<PlaylistTrackTRX> keepList = trackListInfo
                                                            .Where(track => !track.SelectedTrack)
                                                            .OrderBy(track => track.TrackNumber);

                // Create a query for tracks to remove
                IEnumerable<PlaylistTrackTRX> removeList = trackListInfo
                                                            .Where(track => track.SelectedTrack);

                foreach (PlaylistTrackTRX item in removeList)
                {
                    playlistTrackExists = _context.PlaylistTracks
                                            .Where(x => x.Playlist.Name.Equals(playlistName) &&
                                                        x.Playlist.UserName.Equals(userName) &&
                                                        x.TrackId == item.TrackId)
                                            .FirstOrDefault();

                    if (playlistTrackExists != null)
                    {
                        _context.PlaylistTracks.Remove(playlistTrackExists);
                    }
                }

                foreach (PlaylistTrackTRX item in keepList)
                {
                    playlistTrackExists = _context.PlaylistTracks
                                            .Where(x => x.Playlist.Name.Equals(playlistName) &&
                                                        x.Playlist.UserName.Equals(userName) &&
                                                        x.TrackId == item.TrackId)
                                            .FirstOrDefault();

                    if (playlistTrackExists != null)
                    {

                        playlistTrackExists.TrackNumber = trackNumber;
                        _context.PlaylistTracks.Update(playlistTrackExists);

                        // get ready for the next track
                        trackNumber++;
                    }

                    else
                    {
                        var songName = _context.Tracks
                                        .Where(s => s.TrackId == item.TrackId)
                                        .Select(s => s.Name)
                                        .SingleOrDefault();

                        errorList.Add(new Exception($"The track ({songName}) is no longer on file. Please Remove."));
                    }
                }

                if (errorList.Count > 0)
                {
                    throw new AggregateException("Unable to remove selected tracks. Check concerns.", errorList);
                }
                else
                {
                    // All work has been staged
                    _context.SaveChanges();
                }
            }
        }
        

        public void PlaylistTrack_MoveTracks(string playlistName, string userName, List<PlaylistTrackTRX> trackListInfo)
        {
            // local variables
            Playlist playlistExists = null;
            PlaylistTrack playlistTrackExists = null;
            int trackNumber = 1;

            // a container is needed to hold x number of Exception messages
            List<Exception> errorList = new List<Exception>();

            if (string.IsNullOrWhiteSpace(playlistName))
            {
                throw new ArgumentNullException("No playlistName submitted");
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException("No userName submitted");
            }

            var count = trackListInfo.Count();
            if (count == 0)
            {
                throw new ArgumentNullException("No list of tracks were submitted");
            }

            playlistExists = _context.Playlists
                                .Where(x => x.Name.Equals(playlistName) && x.UserName.Equals(userName))
                                .Select(x => x)
                                .FirstOrDefault();

            if (playlistExists == null)
            {
                errorList.Add(new ArgumentException($"Playlist {playlistName} does not exist for this user"));
            }

            else
            {

                // Validation loop to check that the data is indeed a positive number
                // use int.TryParse to check that the value to be tested is a number then check the result of TryParse against the value 1
                int tempNum = 0;

                foreach (var track in trackListInfo)
                {
                    var songName = _context.Tracks
                                        .Where(s => s.TrackId == track.TrackId)
                                        .Select(s => s.Name)
                                        .SingleOrDefault();

                    if (int.TryParse(track.TrackInput.ToString(), out tempNum))
                    {
                        if (tempNum < 1)
                        {
                            errorList.Add(new Exception($"The track ({songName}) re-sequence value needs to be greater than 0"));
                        }
                    }

                    else
                    {
                        errorList.Add(new Exception($"The track ({songName}) re-sequence value needs to be a number. Example: 3"));
                    }
                }

                // Sort the command model data list on the reorg value in ascending order
                // Comparing x to y sorts in ascending order
                // Comparing y to x sorts in descending order
                trackListInfo.Sort((x, y) => x.TrackInput.CompareTo(y.TrackInput));

                // Validate that the new track numbers are unique
                // The collection has been sorted in ascending order, therefore the next number must be equal to or greater than the previous number
                // One could check to see if the next number is +1 of the previous number BUT the reorg loop that does the actual resequence of numbers
                //	will handle that situation. Therefore "holes" in this loop does not matter.

                // Using a for loop here as we are comparing two records and utilizing the indexs of the records
                // Stop comparing at count - 1 as we are comparing 2 records
                for (int i = 0; i < trackListInfo.Count - 1; i++)
                {
                    var songName1 = _context.Tracks
                                        .Where(s => s.TrackId == trackListInfo[i].TrackId)
                                        .Select(s => s.Name)
                                        .SingleOrDefault();

                    var songName2 = _context.Tracks
                                        .Where(s => s.TrackId == trackListInfo[i + 1].TrackId)
                                        .Select(s => s.Name)
                                        .SingleOrDefault();

                    if (trackListInfo[i].TrackInput == trackListInfo[i + 1].TrackInput)
                    {
                        errorList.Add(new Exception($"{songName1} and {songName2} have the same re-sequence value. Resequence numbers must be unique."));
                    }

                }

                foreach (PlaylistTrackTRX item in trackListInfo)
                {
                    playlistTrackExists = _context.PlaylistTracks
                                            .Where(x => x.Playlist.Name.Equals(playlistName) &&
                                                        x.Playlist.UserName.Equals(userName) &&
                                                        x.TrackId == item.TrackId)
                                            .FirstOrDefault();

                    if (playlistTrackExists != null)
                    {

                        playlistTrackExists.TrackNumber = trackNumber;
                        _context.PlaylistTracks.Update(playlistTrackExists);

                        // get ready for the next track
                        trackNumber++;
                    }

                    else
                    {
                        var songName = _context.Tracks
                                        .Where(s => s.TrackId == item.TrackId)
                                        .Select(s => s.Name)
                                        .SingleOrDefault();

                        errorList.Add(new Exception($"The track ({songName}) is no longer on file. Please Remove."));
                    }
                }

                if (errorList.Count > 0)
                {
                    throw new AggregateException("Unable to re-sequence selected tracks. Check concerns.", errorList);
                }
                else
                {
                    // All work has been staged
                    _context.SaveChanges();
                }
            }
        }
        #endregion
    }
}
