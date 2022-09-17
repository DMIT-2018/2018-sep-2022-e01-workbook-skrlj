<Query Kind="Expression">
  <Connection>
    <ID>6cd0a226-8c31-42ad-a971-381fcc6d904b</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
  </Connection>
</Query>

// Aggregates
// .Count() : count the number of instances in the collection
// .Sum(x => ...) : sums (totals) a numeric field (numeric expression) in a collection
// .Min(x => ...) : finds the minimum value of a collection for a field
// .Max(x => ...) : finds the maximum value of a collection for a field
// .Average(x => ...) : finds the average value of a numeric field (numeric expression)

// 						!!! IMPORTANT !!!
// Aggregates work only on a collection of values
// Aggregates DO NOT work on a single instance (non declared collection)
// .Sum, .Min, .Max, .Average MUST have at least one record in their collection
// .Sum and .Average MUST work on numeric fields and the field cannot be NULL

// Syntax: method
// collectionset.aggregate(x => expression)
// collectionset.Select(...).aggregate()
// collectionset.Count() --- .Count() does not contain an expression

// For Sum, Min, Max, and Average: the result is a single value

// You can use multiple aggregates on a single column
// EG.
// .Sum (x => expression).Min(x => expression)

// Find the average playing time (length) of tracks in our music collection

// Average is an aggregate
// What is the collection?
// 		The Tracks table is a collection

Tracks
	.Average(x => x.Milliseconds) // Each x has multiple fields
	
Tracks
	.Select(x => x.Milliseconds).Average() // a single list of numbers

Tracks.Average // aborts because no specific field was reffered to on the tack record

// List all Albums of the sixties showing the title, artist and various aggregates for albums containing tracks
// For each Album, show the number of Tracks, the total price of all tracks, and the average playing length of the album tracks

Albums
	.Where(a => a.Tracks.Count() > 0
				&& (a.ReleaseYear > 1959 && a.ReleaseYear < 1971))
	.Select(a => new 
					{
						Title = a.Title,
						Artist = a.Artist.Name,
						NumberOfTracks = a.Tracks.Count(),
						TotalPrice = a.Tracks.Sum(tr => tr.UnitPrice),
						AverageTrackLength = a.Tracks.Select(tr => tr.Milliseconds).Average()
					}
			)