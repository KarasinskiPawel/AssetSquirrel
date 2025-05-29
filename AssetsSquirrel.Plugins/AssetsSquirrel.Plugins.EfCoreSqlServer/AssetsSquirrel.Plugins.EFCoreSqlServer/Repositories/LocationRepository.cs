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
        private readonly IErrorsRepository errorsRepository;

        public LocationRepository(IDbContextFactory<AssetsSquirrelContext> dbContextFactory, IErrorsRepository errorsRepository)
        {
            this.dbContextFactory = dbContextFactory;
            this.errorsRepository = errorsRepository;
        }

        public async Task<IEnumerable<Location>> GetLocationsAsync(Expression<Func<Location, bool>> where)
        {
            var dbContext = dbContextFactory.CreateDbContext();

            var output = await dbContext.Locations.Where(where)
                .OrderBy(a => a.City)
                .ThenBy(a => a.Street)
                .ToListAsync() ?? Enumerable.Empty<Location>();

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
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "LocationRepository", "AddLocationAsync", e);
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
            catch(Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "LocationRepository", "DeleteLocationAsync", e);
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
            catch(Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "LocationRepository", "UpdateLocationAsync", e);
                return false;
            }

            return true;
        }
    }
}
