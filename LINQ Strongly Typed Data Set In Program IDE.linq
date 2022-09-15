<Query Kind="Program">
  <Connection>
    <ID>a46606c5-cab2-4dc3-8c12-94ec1d3b2254</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
  </Connection>
</Query>

void Main()
{
	// Find songs by partial song name
	// Display the AlbumTitle, Song, and ArtistName
	// Oderby Song
	
	// Pretend that the Main() is the web page
	// Assume that a value was entered into the web page
	// Assume that a post button was pressed 
	// Assume that Main() is the post event
	string inputValue = "dance";
	List<SongList> songCollection = SongsByPartialName(inputValue);
	songCollection.Dump(); // Assuming that Dump() is the web page display
}

// You can define other methods, fields, classes and namespaces here

// C# really enjoys strongly typed data fields whether these fields are primitive data types (int, double, ..ect.) or developer defined data types (class)

// Developer defined data class:
public class SongList 
{
	public string Album {get; set;}
	public string Song {get; set;}
	public string Artist {get; set;}
}

// Imagine the following method exists in a service in your BLL 
// This method recieves the web page parameter value for the query
// This method will need to return a collection
// IQueryable = handles data from SQL, IEnumerable is processed at execution

List<SongList> SongsByPartialName (string partialSongName) 
{
	IEnumerable<SongList> songCollection = Tracks
							.Where (t => t.Name.Contains(partialSongName))
							.OrderBy (t => t.Name)
							.Select (t => new SongList
											{
												Album = t.Album.Title,
												Song = t.Name,
												Artist = t.Album.Artist.Name
											}
									);
	return songCollection.ToList();
}
