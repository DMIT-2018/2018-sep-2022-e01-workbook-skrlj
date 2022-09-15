<Query Kind="Expression">
  <Connection>
    <ID>5b9a15b4-8b4b-4383-a70b-732c1939e0c1</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>WC320-06\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
  </Connection>
</Query>

// List all albums by release label. Any album with no label (null) should be indicated as Unknown
// List Title, Label and Artist Name

// Collection: Albums
// Selective Data: Anonymous data set
// Label (nullable): Either Unknown or LabelName
// Order by the ReleaseLabel field

// Design
// Albums
// Select (new{})
// Fields: Title
//		   Label - using a ternary operator (condition(s) ? trueValue : falseValue)
//		   Artist.Name

// Coding and Testing

Albums
	.Select(x => new
	{
		Title = x.Title,
		Label = x.ReleaseLabel == null ? "Unknown" : x.ReleaseLabel,
		Artist = x.Artist.Name
	})
	.OrderBy(x => x.Label)
	
	
// List all albums showing the Title, ArtistName, Year, and Decade of Release using oldies, 70s, 80s, 90s, or modern. Order by Decade

Albums
	.Select(x => new 
	{
		Title = x.Title,
		Artist = x.Artist.Name,
		Year = x.ReleaseYear,
		DecadeOfRelease = x.ReleaseYear < 1970 ? "Oldies" :
						  x.ReleaseYear < 1980 ? "1970s" :
						  x.ReleaseYear < 1990 ? "1980s" :
						  x.ReleaseYear < 2000 ? "1990s" : "Modern"
	})
	.OrderBy (x => x.Year)