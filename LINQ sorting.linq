<Query Kind="Expression">
  <Connection>
    <ID>b9ded474-0ce3-4d70-b357-147d523bbb61</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>WB320-02\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
  </Connection>
</Query>

// https://dmit-2018.github.io/demos/eRestaurant/linq/LinqPad/expressions.html
// Sorting
// There is a significant difference between query syntax and method syntax

// Query syntax is much like SQL
// Orderby field {[ascending]| descending} [,field...]
// Ascending is the default option

// Method syntax is a series of individual methods
// .OrderBy(x => x.field) (first field only)
// .OrderByDescending(x => x.field)
// .ThenBy(x => x.field) (each following field)
// .ThenByDescending(x => x.field)

// Find all of the album tracks for the band "Queen". Order the tracks by the track name alphabetically

// Query syntax
from x in Tracks
where x.Album.Artist.Name.Contains("Queen")
orderby x.AlbumId, x.Name
select x

Tracks
	.Where (x => x.Album.Artist.Name.Contains("Queen"))
	.OrderBy (x => x.Album.Title)
	.ThenBy (x => x.Name)
	
// Speed of the processing is the only thing that is effected by interchanging the Where and OrderBy clauses. ThenBy must always follow the OrderBy clause
	
Tracks
	.OrderBy (x => x.Album.Title)
	.ThenBy (x => x.Name)
	.Where (x => x.Album.Artist.Name.Contains("Queen"))
