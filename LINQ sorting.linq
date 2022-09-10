<Query Kind="Expression">
  <Connection>
    <ID>54bf9502-9daf-4093-88e8-7177c12aaaaa</ID>
    <NamingService>2</NamingService>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <AttachFileName>&lt;ApplicationData&gt;\LINQPad\ChinookDemoDb.sqlite</AttachFileName>
    <DisplayName>Demo database (SQLite)</DisplayName>
    <DriverData>
      <PreserveNumeric1>true</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.Sqlite</EFProvider>
      <MapSQLiteDateTimes>true</MapSQLiteDateTimes>
      <MapSQLiteBooleans>true</MapSQLiteBooleans>
    </DriverData>
  </Connection>
</Query>

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
