using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using ChinookSystem.DAL;
using ChinookSystem.BLL;
#endregion

namespace ChinookSystem
{
    // Needs to be a public so it can be used outside of this project
    // Needs to be static 
    public static class ChinookExtensions
    {
        // Method name can be anything but it MUST match the builder.Services method call in Program.cs
        // ex builder.Services.***ChinookSystemBackendDependencies***(options => options.UseSqlServer(connectionStringChinook));

        // The first parameter in the method is the class that we are attempting to add an extension to
        // The second parameter is options value in the call statement. The options value is receiving the connectionString for the application
        public static void ChinookSystemBackendDependencies(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            // Register the DbContext class with the service collection
            services.AddDbContext<ChinookContext>(options);

            // Add any services that we create in the class library using .AddTransient<serviceClassName>(...)
            services.AddTransient<TrackServices>((serviceProvider) =>
            {
                // Retrieve the registered DbContext done with AddDbContext
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                return new TrackServices(context);
            });

            services.AddTransient<PlaylistTrackServices>((serviceProvider) =>
            {
                // Retrieve the registered DbContext done with AddDbContext
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                return new PlaylistTrackServices(context);
            });

            services.AddTransient<PlaylistServices>((serviceProvider) =>
            {
                // Retrieve the registered DbContext done with AddDbContext
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                return new PlaylistServices(context);
            });

            services.AddTransient<MediaTypeServies>((serviceProvider) =>
            {
                // Retrieve the registered DbContext done with AddDbContext
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                return new MediaTypeServies(context);
            });

            services.AddTransient<GenreServices>((serviceProvider) =>
            {
                // Retrieve the registered DbContext done with AddDbContext
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                return new GenreServices(context);
            });

            services.AddTransient<EmployeeServices>((serviceProvider) =>
            {
                // Retrieve the registered DbContext done with AddDbContext
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                return new EmployeeServices(context);
            });

            services.AddTransient<CustomerServices>((serviceProvider) =>
            {
                // Retrieve the registered DbContext done with AddDbContext
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                return new CustomerServices(context);
            });

            services.AddTransient<ArtistServices>((serviceProvider) =>
            {
                // Retrieve the registered DbContext done with AddDbContext
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                return new ArtistServices(context);
            });

            services.AddTransient<AlbumServices>((serviceProvider) =>
            {
                // Retrieve the registered DbContext done with AddDbContext
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                return new AlbumServices(context);
            });
        }
    }
}
