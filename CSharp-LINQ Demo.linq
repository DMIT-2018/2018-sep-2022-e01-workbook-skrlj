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
		
		//On the webpage, the POST method would have already have access to the BindProperty variables containing the input values
		//playlistName = "hansenbtest";
		//int trackId = 756;
		
		// the POST would call the service method to process the data
		//PlaylistTrack_AddTrack(playlistName, userName, trackId);
		
		playlistName = "hansenbtest";
		List<PlaylistTrackTRX> removeTrackInfo = new List<PlaylistTrackTRX>();
		removeTrackInfo.Add(new PlaylistTrackTRX()
								{
									SelectedTrack = true,
									TrackId = 793,
									TrackNumber = 1,
									TrackInput = 4
								}
							);
		
		removeTrackInfo.Add(new PlaylistTrackTRX()
								{
									SelectedTrack = true,
									TrackId = 822,
									TrackNumber = 2,
									TrackInput = 3
								}
							);
							
		removeTrackInfo.Add(new PlaylistTrackTRX()
								{
									SelectedTrack = false,
									TrackId = 543,
									TrackNumber = 3,
									TrackInput = 2
								}
							);
							
		removeTrackInfo.Add(new PlaylistTrackTRX()
								{
									SelectedTrack = false,
									TrackId = 756,
									TrackNumber = 4,
									TrackInput = 1
								}
							);
											
		// call the service method to process the data
		// PlaylistTrack_RemoveTracks(playlistName, userName, removeTrackInfo);
		
		// call the service method to process the data
		 PlaylistTrack_MoveTracks(playlistName, userName, removeTrackInfo);
		
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
	
	catch (AggregateException ex)
	{
	 	// Having collected a number of errors, each error should be dumped to a seperate line
		foreach(var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}
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

#region Command Model for Remove and Move Track
public class PlaylistTrackTRX
{
   public bool SelectedTrack {get; set;}
   public int TrackId {get; set;}
   public int TrackNumber {get; set;}
   public int TrackInput {get; set;}
}
#endregion

// General method to drill down into an exception to obtain the InnerException where your actual error is detailed
private Exception GetInnerException(Exception ex)
{
	while (ex.InnerException != null)
		ex = ex.InnerException;
	return ex;
}

// Pretend to be the class library project
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
												x.Playlist.UserName.Equals(userName)
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
	SaveChanges();
}

public void PlaylistTrack_RemoveTracks(string playlistName, string userName, List<PlaylistTrackTRX> trackListInfo)
{
	// local variables
	Playlists playlistExists = null;
	PlaylistTracks playlistTrackExists = null;
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
	
	playlistExists = Playlists
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
		
		foreach(PlaylistTrackTRX item in removeList)
		{
			playlistTrackExists = PlaylistTracks
									.Where(x => x.Playlist.Name.Equals(playlistName) &&
												x.Playlist.UserName.Equals(userName) &&
												x.TrackId == item.TrackId)
									.FirstOrDefault();
			
			if (playlistTrackExists != null)
			{
				PlaylistTracks.Remove(playlistTrackExists);
			}
		}
		
		foreach(PlaylistTrackTRX item in keepList)
		{
			playlistTrackExists = PlaylistTracks
									.Where(x => x.Playlist.Name.Equals(playlistName) &&
												x.Playlist.UserName.Equals(userName) &&
												x.TrackId == item.TrackId)
									.FirstOrDefault();
			
			if (playlistTrackExists != null)
			{
				
				playlistTrackExists.TrackNumber = trackNumber;
				PlaylistTracks.Update(playlistTrackExists);
				
				// get ready for the next track
				trackNumber++;
			}
			
			else
			{
				var songName = Tracks
								.Where(s => s.TrackId == item.TrackId)
								.Select(s => s.Name)
								.SingleOrDefault();
								
				errorList.Add(new Exception($"The track ({songName}) is no longer on file. Please Remove."));			
			}
		}
		
		if(errorList.Count > 0)
		{
			throw new AggregateException("Unable to remove selected tracks. Check concerns.", errorList);
		}
		else
		{
			// All work has been staged
			SaveChanges();
		}
	}
}
#endregion

#region
public void PlaylistTrack_MoveTracks(string playlistName, string userName, List<PlaylistTrackTRX> trackListInfo)
{
	// local variables
	Playlists playlistExists = null;
	PlaylistTracks playlistTrackExists = null;
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
	
	playlistExists = Playlists
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
		
		foreach(var track in trackListInfo)
		{
			var songName = Tracks
								.Where(s => s.TrackId == track.TrackId)
								.Select(s => s.Name)
								.SingleOrDefault();
			
			if(int.TryParse(track.TrackInput.ToString(), out tempNum))
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
		trackListInfo.Sort((x,y) => x.TrackInput.CompareTo(y.TrackInput));
		
		// Validate that the new track numbers are unique
		// The collection has been sorted in ascending order, therefore the next number must be equal to or greater than the previous number
		// One could check to see if the next number is +1 of the previous number BUT the reorg loop that does the actual resequence of numbers
		//	will handle that situation. Therefore "holes" in this loop does not matter.
		
		// Using a for loop here as we are comparing two records and utilizing the indexs of the records
		// Stop comparing at count - 1 as we are comparing 2 records
		for(int i = 0; i < trackListInfo.Count - 1; i++)
		{
			var songName1 = Tracks
								.Where(s => s.TrackId == trackListInfo[i].TrackId)
								.Select(s => s.Name)
								.SingleOrDefault();
								
			var songName2 = Tracks
								.Where(s => s.TrackId == trackListInfo[i + 1].TrackId)
								.Select(s => s.Name)
								.SingleOrDefault();
							
			if (trackListInfo[i].TrackInput == trackListInfo[i + 1].TrackInput)
			{
				errorList.Add(new Exception($"{songName1} and {songName2} have the same re-sequence value. Resequence numbers must be unique."));
			}
						
		}
		
		foreach(PlaylistTrackTRX item in trackListInfo)
		{
			playlistTrackExists = PlaylistTracks
									.Where(x => x.Playlist.Name.Equals(playlistName) &&
												x.Playlist.UserName.Equals(userName) &&
												x.TrackId == item.TrackId)
									.FirstOrDefault();
			
			if (playlistTrackExists != null)
			{
				
				playlistTrackExists.TrackNumber = trackNumber;
				PlaylistTracks.Update(playlistTrackExists);
				
				// get ready for the next track
				trackNumber++;
			}
			
			else
			{
				var songName = Tracks
								.Where(s => s.TrackId == item.TrackId)
								.Select(s => s.Name)
								.SingleOrDefault();
								
				errorList.Add(new Exception($"The track ({songName}) is no longer on file. Please Remove."));			
			}
		}
		
		if(errorList.Count > 0)
		{
			throw new AggregateException("Unable to re-sequence selected tracks. Check concerns.", errorList);
		}
		else
		{
			// All work has been staged
			SaveChanges();
		}
	}
}
#endregion