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
	string searchArg = "Deep";
	string searchBy = "Artist";
	List<TrackSelection> trackList = Track_FetchTracksBy(searchArg, searchBy);
	trackList.Dump();
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

// Pretend that the 
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