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

Albums

// Query Syntax to list all records in an entity (table, collection)
from aRowOnCollection in Albums
select aRowOnCollection

// Method syntax to list all records in an entity
Albums 
   .Select (aRowOnCollection => aRowOnCollection)

// Where
// Filter Method
// the conditions are setup as you would in C#
// beware that LinqPad may not like some C# syntax (DateTime)
// beware that Linq is converted to SQL which may not like certain C# syntax because SQL could not convert

// syntax
// notice that the method syntax makes use of the lambda expression
// lambdas are common when performing Linq with the method syntax
// .Where(lambda expression)
// lambda can be thought of as "do the following"
// .Where(x => condition [logical operator condition2 ...]
// .Where(x => Bytes > 350000)

Tracks
	.Where(x => x.Bytes > 700000000)
	
// or

from x in Tracks
where x.Bytes > 700000000
select x

// Find all the albums of the artist Queen
// concerns: the artist name is in another table
// 			 in an sql Select you would be using an inner join
// 			 in Linq you do not need to specify your inner join
//			 instead, use the "navigational properties" of your entity to generate the relationship

Albums
	.Where(x => x.Artist.Name.Contains("Queen"))