<Query Kind="Program">
  <Connection>
    <ID>fa389fcc-ed69-4b31-8278-3ade764f17c3</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
  </Connection>
</Query>

// .IQueryable() = results of a SQL query
// .IEnumerable() = results of any collection in memory

// .ToList() is helpful as its brings data in memory for further processing. Might be a solution to some problems

void Main()
{
	// Conversions
	// Collections we will look at are IQueryable, IEnumerable, and List
	
	// Display all Albums and their Tracks
	// Display the AlbumTitle, ArtistName, AlbumTracks
	// For each track, show the SongName and Playtime
	// Show only Albums with 25 or more tracks
	
	List<AlbumTracks> AlbumList = Albums
						.Where(a => a.Tracks.Count() >= 25)
						.Select(a => new AlbumTracks
										{
											Title = a.Title,
											Artist = a.Artist.Name,
											Songs = a.Tracks
														.Select(tr => new SongItem
																		{
																			Song = tr.Name,
																			Playtime = tr.Milliseconds / 1000.0
																		}
														       )
														.ToList()
										}
								)
						.ToList()
						//.Dump()
						;
						
						
						
	// Using .FirstOrDefault
	// Previously used to check to see if a record existed in a BLL service method
	
	// Find the first Album by Deep Purple
	
	var artistParam = "Deep Purple";
	var firstAlbum = Albums
						.Where(a => a.Artist.Name.Equals(artistParam))
						.Select(a => a)
						.OrderBy(a => a.ReleaseYear)
						.FirstOrDefault()
						//.Dump()
						;
	//if (firstAlbum != null) {
	//	firstAlbum.Dump();
	//}
	//else {
	//	Console.WriteLine($"No albums found for artist {artistParam}");
	//}
	
	// .Distinct()
	// Removes duplicate reported lines
	
	// Get a list of Customer Countries
	var resultsDistinct = Customers
							.OrderBy(c => c.Country)
							.Select(c => c.Country)
							.Distinct()
							//.Dump()
							;
							
							
	// .Take() and .Skip()
	// Previously, when we wanted to use the supplied Paginator, the query method was to return ONLY the needed records for the 
	// 	display, NOT the entire collection
	// A) The query was executed returning a collection of size x
	// B) Obtained the total count (x) of return records
	// C) Calculated the number of records to skip (pageNumber - 1) * pageSize
	// D) On the return method statement, we used:
	// 		return variableName.Skip(rowsSkipped).Take(pagesize).ToList()
	
	// Union
	// Rules in LINQ are the same as in SQL
	// Result is the same as SQL, combine seperate collections into one.
	// Syntax: (queryA.Union(queryB)[.Union(query...)])
	// Rules:
	//	1) Number of columns must be the same
	// 	2) Column data types must be the same
	//  3) Ordering shoul dbe done as a method after the last Union
	
	var resultsUnionA = (Albums
							.Where(x => x.Tracks.Count() == 0)
							.Select(x => new 
											{
												Title = x.Title,
												TotalTracks = 0,
												// m after the number makes the 
												TotalCost = 0.00m,
												AverageLength = 0.00d

											}
								   )
						)
						.Union(Albums
									.Where(x => x.Tracks.Count() > 0)
									.Select(x => new 
													{
														Title = x.Title,
														TotalTracks = x.Tracks.Count(),
														TotalCost = x.Tracks.Sum(tr => tr.UnitPrice),
														AverageLength = x.Tracks.Average(tr => tr.Milliseconds)
													}
										   )
							   )
						.OrderBy(x => x.TotalTracks)
						.Dump()	
						;
						
		
}

// You can define other methods, fields, classes and namespaces here

public class SongItem
{
	public string Song{get; set;}
	public double Playtime{get; set;}
}

public class AlbumTracks
{
	public string Title {get; set;}
	public string Artist {get; set;}
	public List<SongItem> Songs {get; set;}
}