<Query Kind="Statements">
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