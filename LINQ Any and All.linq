<Query Kind="Statements">
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

// Any and All
// These filter tests return a true or false condition
// They work at the complete collection level

Genres
	.Count()
	//.Dump()
	;
// Count is 25 Genres

// Show Genres that have tracks which are not on any playlist
Genres
	.Where(g => g.Tracks.Any(tr => tr.PlaylistTracks.Count() == 0))
	.Select(g => g)
	//.Dump()
	;

// Show Genres that have all the tracks appearing at least once on a playlist
Genres
	.Where(g => g.Tracks.All(tr => tr.PlaylistTracks.Count() > 0))
	.Select(g => g)
	//.Dump()
	;
	
// There maybe times that using a !Any() -> All(!relationship)
// Same goes for !All -> Any(!relationship)

// Using All and Any in comparing 2 collections. If the collection is not a complex record, there is a LINQ method called .Except
//	that can be used to solve the query.

// Compare the track collection of 2 people, using All and Any

// Roberto Almeida and Michelle Brooks

var almeida = PlaylistTracks
					.Where(x => x.Playlist.UserName.Contains("AlmeidaR"))
					.Select(x => new {
										Song = x.Track.Name,
										Genre = x.Track.Genre.Name,
										ID = x.TrackId,
										Artist = x.Track.Album.Artist.Name
									 }
						    )
					.Distinct()
					.OrderBy(x => x.Song)
					//.Dump()
					// 110 tracks
					;
					
var brooks = PlaylistTracks
					.Where(x => x.Playlist.UserName.Contains("BrooksM"))
					.Select(x => new {
										Song = x.Track.Name,
										Genre = x.Track.Genre.Name,
										ID = x.TrackId,
										Artist = x.Track.Album.Artist.Name
									 }
						    )
					.Distinct()
					.OrderBy(x => x.Song)
					//.Dump()
					// 88 Tracks
					;
					
// List the tracks that BOTH Roberto and Michelle like by comparing the 2 data sets. Data in list A that is also in list B
// Assume that Roberto is list A and list B is Michelle
// Assume list A is what you wish to report from
// Assume list B is what you wish to compare

// What songs does Roberto like but not Michelle
var c1 = almeida
			.Where(rob => !brooks.Any(mic => mic.ID == rob.ID))
			.OrderBy(rob => rob.Song)
			//.Dump()
			;
			
var c2 = almeida
			.Where(rob => brooks.All(mic => mic.ID != rob.ID))
			.OrderBy(rob => rob.Song)
			//.Dump()
			;

var v3 = brooks
			.Where(mic => almeida.All(rob => rob.ID != mic.ID))
			.OrderBy(mic => mic.Song)
			//.Dump()
			;
			
// What songs do both Michelle and Roberto like
var c4 = brooks
			.Where(mic => almeida.Any(rob => rob.ID == mic.ID))
			.OrderBy(mic => mic.Song)
			//.Dump()
			;

