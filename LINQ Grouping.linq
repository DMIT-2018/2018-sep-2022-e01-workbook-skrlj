<Query Kind="Expression">
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

// Grouping

// When you create a group, it builds two componenets: 
// 1) A key component (deciding criteria value(s) defining the group.
//    Reference this component using the groupname.Key[.propertyName]
//			1 value for key: groupName.Key
//			n values for key = groupName.Key.propertyName
// Property == Field == Attribute == Value
//
// 2) Data of the grouped items - the raw instances of the collection

// Ways to group:
// 1) By a single column (field, attribute, value, property)
//		groupName.Key
// 2) By a set of columns (anonymous data set)
//		groupName.Key.propertyName
// 3) By an entity (entity name/ nav property) resulting in an anonymous data set
//		groupName.Key.propertyName

// Concept processing
// Start with a "pile" of data (original collection prior to grouping)
// Specify the grouping property(ies)
// Result of the group operation will be to "place the data into smaller piles"
// The "piles" are dependant on the grouping property(ies) value(s)
// The grouping property(ies) become the Key
// The individual instances are the data in the smaller piles
// The entire individual instance of the original collection is placed in the smaller pile
// Manipulate each of the "smaller piles" using your LINQ commands


// Grouping is different than ordering
// Ordering is the final re-sequencing of a collection for display
// Grouping re-organizes a collection into seperate usually smaller collections for further processing (ie aggregates)

// Grouping is an excellent way to organize your data, especially if you need to process data on a property that is not a relative Key
// Such as a foreign key which forms a "natural" group using the navigational properties



// Display Albums by ReleaseYear
// This request does NOT need grouping
// This request is an ordering of output: OrderBy
// This ordering affects only display

Albums
	.OrderBy(a => a.ReleaseYear)
	

// Display Albums grouped by ReleaseYear
// Explicit request to breakup the display into desired "piles"

Albums
	.GroupBy(a => a.ReleaseYear)
	
// Processing on the groups created by the Group command

// Display the number of albums produced each year
Albums
	.GroupBy(a => a.ReleaseYear)
	.Select(eachGroupPile => new {
									Year = eachGroupPile.Key,
									NumberOfAlbums = eachGroupPile.Count()
								 }
			)

// Display the number of albums produced each year
// List only the years which have more than 10 albums

Albums
	.GroupBy(album => album.ReleaseYear)
	//.Where(group => group.Count() > 10) --- Filtering against each group pile
	.Select(groupPile => new {
									Year = groupPile.Key,
									NumberOfAlbums = groupPile.Count()
								 }
			)
	.Where(x => x.NumberOfAlbums > 10) // Filtering against the output of the .Select(...) command


// Use a multiple set of properties to form the group
// Include a nested query to report on the small pile group

// Display Albums grouped by ReleaseLabel, ReleaseYear and number of Albums.
// List only the years with 3 or more albums released.
// For each album, display the Title, Artist, Number of Tracks, and ReleaseYear

Albums
	.GroupBy(album => new{album.ReleaseLabel, album.ReleaseYear})
	.Where(group => group.Count() > 2)
	.Select(groupPile => new {
									Label = groupPile.Key.ReleaseLabel,
									Year = groupPile.Key.ReleaseYear,
									NumberOfAlbums = groupPile.Count(),
									AlbumItems = groupPile
													.Select(egPInstance => new {
																					Title = egPInstance.Title,
																					Artist = egPInstance.Artist.Name,
																					YearOfAlbum = egPInstance.ReleaseYear,
																					TrackCount = egPInstance.Tracks.Count()					
																			   }
														   )
								 }
			)














