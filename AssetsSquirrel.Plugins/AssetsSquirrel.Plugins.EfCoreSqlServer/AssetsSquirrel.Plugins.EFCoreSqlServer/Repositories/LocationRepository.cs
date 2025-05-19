using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly IDbContextFactory<AssetsSquirrelContext> dbContextFactory;

        public LocationRepository(IDbContextFactory<AssetsSquirrelContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }
        public async Task AddLocationAsync(Location location)
        {
            if(location is not null)
            {
                var dbContext = dbContextFactory.CreateDbContext();

                dbContext.Locations.Add(location);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteLocationAsync(Location location)
        {
            if(location is not null)
            {
                var dbContext = dbContextFactory?.CreateDbContext();

                dbContext.Locations.Remove(location);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Location>> GetLocationsAsync(Expression<Func<Location, bool>> where)
        {
            var dbContext = dbContextFactory.CreateDbContext();

            var output = await dbContext.Locations.Where(where).ToListAsync() ?? Enumerable.Empty<Location>();

            return output;
        }

        public async Task UpdateLocationAsync(Location location)
        {
            if(location is not null)
            {
                var dbContext = dbContextFactory.CreateDbContext();

                dbContext.Locations.Add(location);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
