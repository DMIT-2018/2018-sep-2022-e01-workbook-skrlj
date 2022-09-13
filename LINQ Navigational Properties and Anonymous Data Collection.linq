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
// Using Navigational Properties and Anonymous Data Sets (Collections)

// Find all albums released in the 90's (1990-1999)
// Order the albums by ascending year and then alphabetically by album title
// Display the year, title, artist name, and release label
// Concerns: Not all properties of album are to be displayed
// 			 The order of the properties are to be displayed in a different sequence then the 
// definition of the properties on the entitiy Album
//			 The artist name is Not on the Album table BUT is on the Artist table

// The anonymous data instance is defined within the Select by declared fields (properties)
// The order of the fields on the new defined instance will be done in specifying the properties 
// of the anonymous data collection

Albums
	.Where(oldies => oldies.ReleaseYear > 1989 && oldies.ReleaseYear < 2000)
	//.OrderBy (oldies => oldies.ReleaseYear)
	//.ThenBy (oldies => oldies.Title)
	.Select (
				oldies =>
					new
					{
						Year = oldies.ReleaseYear,
						Title = oldies.Title,
						ArtistName = oldies.Artist.Name,
						ReleaseLabel = oldies.ReleaseLabel
					}
			)
	.OrderBy (oldies => oldies.Year) // Year is in the anonymous data type definition, ReleaseYear is not
	.ThenBy (oldies => oldies.Title)

// *** Remember what comes out of one method, goes into the next method ***
