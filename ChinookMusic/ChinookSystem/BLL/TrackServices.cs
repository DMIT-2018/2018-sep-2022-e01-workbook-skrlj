using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using ChinookSystem.DAL;
using ChinookSystem.ViewModels;
#endregion

namespace ChinookSystem.BLL
{
    // Public as it will be called outside the application
    public class TrackServices
    {
        #region Constructor for Context Dependencies Dependency
        private readonly ChinookContext _context;

        internal TrackServices(ChinookContext context)
        {
            _context = context;
        }
        #endregion

        #region Queries
        public List<TrackSelection> Track_FetchTracksBy(string searchArg, string searchBy, int pageNumber, int pageSize, out int totalCount)
        {
            if (string.IsNullOrWhiteSpace(searchArg))
            {
                throw new ArgumentNullException("No search value submitted");
            }

            if (string.IsNullOrWhiteSpace(searchBy))
            {
                throw new ArgumentNullException("No search style submitted");
            }

            // Needed to add _context before the Tracks as context is an object of the database
            IEnumerable<TrackSelection> results = _context.Tracks
                                                    .Where(x => (x.Album.Artist.Name.Contains(searchArg) && searchBy.Equals("Artist")) ||
                                                                (x.Album.Title.Contains(searchArg) && searchBy.Equals("Album")))
                                                    .Select(x => new TrackSelection
                                                    {
                                                        TrackId = x.TrackId,
                                                        SongName = x.Name,
                                                        AlbumTitle = x.Album.Title,
                                                        ArtistName = x.Album.Artist.Name,
                                                        Milliseconds = x.Milliseconds,
                                                        Price = x.UnitPrice
                                                    })
                                                    .OrderBy(x => x.SongName);
            totalCount = results.Count();
            int rowsSkipped = (pageNumber - 1) * pageSize;
            return results.Skip(rowsSkipped).Take(pageSize).ToList();
        }
        #endregion
    }
}