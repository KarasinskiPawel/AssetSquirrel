using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories
{
    public class ManufacturersRepository : IManufacturersRepository
    {
        private readonly IDbContextFactory<AssetsSquirrelContext> dbContextFactory;
        private readonly IErrorsRepository errorsRepository;

        public ManufacturersRepository(IDbContextFactory<AssetsSquirrelContext> dbContextFactory, IErrorsRepository errorsRepository)
        {
            this.dbContextFactory = dbContextFactory;
            this.errorsRepository = errorsRepository;
        }

        public async Task<IEnumerable<Manufacturer>> GetManufacturersAsync(Expression<Func<Manufacturer, bool>> where)
        {
            var dbContext = dbContextFactory.CreateDbContext();

            var output = await dbContext.Manufacturers
                .OrderBy(a => a.Name)
                .ToListAsync();

            return output;
        }

        public async Task<bool> AddManufacturerAsync(Manufacturer manufacturer)
        {
            try
            {
                if (manufacturer is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();

                    dbContext.Manufacturers.Add(manufacturer);
                    dbContext.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "ManufacturersRepository", "AddManufacturerAsync", e);
                return false;
            }
        }

        public async Task<bool> DeleteManufacturerAsync(Manufacturer manufacturer)
        {
            try
            {
                if (manufacturer is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();

                    dbContext.Manufacturers.Remove(manufacturer);
                    dbContext.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "ManufacturersRepository", "DeleteManufacturerAsync", e);
                return false;
            }
        }

        public async Task<bool> UpdateManufacturerAsync(Manufacturer manufacturer)
        {
            try
            {
                if (manufacturer is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();

                    dbContext.Manufacturers.Update(manufacturer);
                    dbContext.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "ManufacturersRepository", "UpdateManufacturerAsync", e);
                return false;
            }
        }
    }
}
