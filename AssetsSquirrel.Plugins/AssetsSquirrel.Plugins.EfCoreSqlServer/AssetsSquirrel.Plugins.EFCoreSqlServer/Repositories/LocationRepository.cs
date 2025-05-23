using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public async Task<IEnumerable<Location>> GetLocationsAsync(Expression<Func<Location, bool>> where)
        {
            var dbContext = dbContextFactory.CreateDbContext();

            var output = await dbContext.Locations.Where(where).ToListAsync() ?? Enumerable.Empty<Location>().ToList();

            return output;
        }
        public async Task<bool> AddLocationAsync(Location location)
        {
            try
            {
                if (location is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();

                    dbContext.Locations.Add(location);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteLocationAsync(Location location)
        {
            try
            {
                if (location is not null)
                {
                    var dbContext = dbContextFactory?.CreateDbContext();

                    dbContext.Locations.Remove(location);
                    await dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateLocationAsync(Location location)
        {
            try
            {
                if (location is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();

                    dbContext.Locations.Update(location);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
