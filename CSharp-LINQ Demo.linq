<Query Kind="Program">
  <Connection>
    <ID>93195052-47f3-447c-a933-5444a37b4abd</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Server>.\SQLEXPRESS</Server>
    <DisplayName>Chinook-Entity</DisplayName>
    <Database>Chinook</Database>
    <DriverData>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	// Main is going to represent the web page POST method
	try
	{
	
	
		// Coded and tested the FetchTracksBy query
		string searchArg = "Deep";
		string searchBy = "Artist";
		List<TrackSelection> trackList = Track_FetchTracksBy(searchArg, searchBy);
		//trackList.Dump();
		
		// Coded and tested the FetchPlaylist query
		string playlistName = "hansenb1";
		string userName = "HansenB"; // this is an user name which will come from O/S via security
		List<PlaylistTrackInfo> playlist = PlaylistTrack_FetchPlaylist(playlistName, userName);
		//playlist.Dump();
		
		// Coded and tested the Add_Track TRX (transaction)
		// The command method will recieve no collection but will recieve individual arguments
		// Arguments: trackId, playlistName, userName
		// Test tracks
		// 793 A castle full of Rascals
		// 822 A Twist in the Tail
		// 543 Burn
		// 756 Child in Time
		
		// On the webpage, the POST method would have already have access to the BindProperty variables containing the input values
		playlistName = "hansenbtest";
		int trackId = 793;
		
		// the POST would call the service method to process the data
		PlaylistTrack_AddTrack(playlistName, userName, trackId);
		
		// Once the service method is complete, the webpage would refresh to update the playlist
		playlist = PlaylistTrack_FetchPlaylist(playlistName, userName);
		playlist.Dump();
		
	}
	
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	
	catch (ArgumentException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}

}

#region CQRS Queries
public class TrackSelection
{
   public int TrackId {get; set;}
   public string SongName {get; set;}
   public string AlbumTitle {get; set;}
   public string ArtistName {get; set;}
   public int Milliseconds {get; set;}
   public decimal Price {get; set;}
}

public class PlaylistTrackInfo
{
   public int TrackId {get; set;}
   public int TrackNumber {get; set;}
   public string SongName {get; set;}
   public int Milliseconds {get; set;}
}
#endregion

// General method to drill down into an exception to obtain the InnerException where your actual error is detailed
private Exception GetInnerException(Exception ex)
{
	while (ex.InnerException != null)
		ex = ex.InnerException;
	return ex;
}

// Orentedn to be the class library project
#region TrackServices class
public List<TrackSelection> Track_FetchTracksBy(string searchArg, string searchBy)
{
	if (string.IsNullOrWhiteSpace(searchArg))
	{
		throw new ArgumentNullException("No search value submitted");
	}
	
	if (string.IsNullOrWhiteSpace(searchBy))
	{
		throw new ArgumentNullException("No search style submitted");
	}
	
	IEnumerable<TrackSelection> results = Tracks
											.Where(x => (x.Album.Artist.Name.Contains(searchArg) && searchBy.Equals("Artist")) || 
														(x.Album.Artist.Name.Contains(searchArg) && searchBy.Equals("Album")))
											.Select(x => new TrackSelection
															{
																TrackId = x.TrackId,
																SongName = x.Name,
																AlbumTitle = x.Album.Title,
																ArtistName = x.Album.Artist.Name,
																Milliseconds = x.Milliseconds,
																Price = x.UnitPrice
															});
	
	return results.ToList();
}
#endregion

#region TrackServices class
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
	
	IEnumerable<PlaylistTrackInfo> results = PlaylistTracks
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

#region
void PlaylistTrack_AddTrack(string playlistName, string userName, int trackId)
{
	// locals
	Tracks trackExists = null;
	Playlists playlistExists = null;
	PlaylistTracks playlistTrackExists = null;
	int trackNumber = 0;
	
	if (string.IsNullOrWhiteSpace(playlistName))
	{
		throw new ArgumentNullException("No playlistName submitted");
	}
	
	if (string.IsNullOrWhiteSpace(userName))
	{
		throw new ArgumentNullException("No userName submitted");
	}
	
	trackExists = Tracks
					.Where(x => x.TrackId == trackId)
					.Select(x => x)
					.FirstOrDefault();
					
	if (trackExists == null)
	{
		throw new ArgumentException("Selected track no longer on file. Refresh track table.");
	}
	
	// Playlist names must be unique within a user
	playlistExists = Playlists
						.Where(x => x.Name.Equals(playlistName) && x.UserName.Equals(userName))
						.Select(x => x)
						.FirstOrDefault();
					
	if (playlistExists == null)
	{
		playlistExists = new Playlists()
		{
			Name = playlistName,
			UserName = userName
		};
		
		// Staging the new playlist record
		Playlists.Add(playlistExists);
		trackNumber = 1;
	}
	
	else
	{
		// BR: a track may only exist once on a playlist
		playlistTrackExists = PlaylistTracks
									.Where(x => x.Playlist.Name.Equals(playlistName) &&
												x.Playlist.UserName.Equals(userName) &&
												x.TrackId == trackId
											)
									.Select(x => x)
									.FirstOrDefault();
		
		if (playlistTrackExists == null)
		{
			// Generate the next track number
			trackNumber = PlaylistTracks
								.Where(x => x.Playlist.Name.Equals(playlistName) &&
												x.Playlist.UserName.Equals(userName) &&
												x.TrackId == trackId
										)
								.Count();
			trackNumber++;
		}
		
		else
		{
			var songName = Tracks
							.Where(s => s.TrackId == trackId)
							.Select(s => s.Name)
							.SingleOrDefault();
							
			throw new Exception($"Selected track ({songName}) already exists on the playlist.");
		}
	}
	
	// Processing to stage the new track to the playlist
	playlistTrackExists = new PlaylistTracks();
	
	// Load the data to the new instance of playlistTrack
	playlistTrackExists.TrackNumber = trackNumber;
	playlistTrackExists.TrackId = trackId;
	playlistTrackExists.PlaylistId = 
	
}
#endregion