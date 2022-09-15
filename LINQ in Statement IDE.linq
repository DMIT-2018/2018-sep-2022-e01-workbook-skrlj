<Query Kind="Statements">
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

// https://dmit-2018.github.io/demos/eRestaurant/linq/LinqPad/expressions.html
// The Statement ide
// This environment expects the use of C# statment grammar
// The results of a query is NOT automatically displayed as in the expression environment
// To display the results, you need to .Dump() the variable holding the data result
// ! Important ! .Dump() is a LINQPad method, NOT a C# method
// Within the statement environment one can run ALL the queries in one execution

var qSyntaxList = from aRowOnCollection in Albums
				  select aRowOnCollection;
qSyntaxList.Dump();

var mSyntaxList = Albums 
   					.Select (aRowOnCollection => aRowOnCollection)
					.Dump();
//mSyntaxList.Dump();

var mQueenAlbumList = Albums
						.Where(x => x.Artist.Name.Contains("Queen"))
						.Dump();