using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
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
    public class SuppilersRepository : ISuppilersRepository
    {
        private readonly IDbContextFactory<AssetsSquirrelContext> dbContextFactory;
        private readonly IErrorsRepository errorsRepository;

        public SuppilersRepository(IDbContextFactory<AssetsSquirrelContext> dbContextFactory, IErrorsRepository errorsRepository)
        {
            this.dbContextFactory = dbContextFactory;
            this.errorsRepository = errorsRepository;
        }
        public async Task<IEnumerable<Suppiler>> GetSuppilersAsync(Expression<Func<Suppiler, bool>> where)
        {
            var dbContext = dbContextFactory.CreateDbContext();

            var output = await dbContext.Suppilers.Where(where).ToListAsync() ?? Enumerable.Empty<Suppiler>().ToList();

            return output;
        }

        public async Task<bool> AddSuppilerAsync(Suppiler suppiler)
        {
            try
            {
                if(suppiler is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();

                    dbContext.Suppilers.Add(suppiler);
                    await dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "SuppilersRepository", "AddSuppilerAsync", e);
                return false;
            }
        }

        public async Task<bool> DeleteSuppilerAsync(Suppiler suppiler)
        {
            try
            {
                if(suppiler is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();

                    dbContext?.Suppilers.Remove(suppiler);
                    await dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "SuppilersRepository", "DeleteSuppilerAsync", e);
                return false;
            }
        }

        public async Task<bool> UpdateSuppilerAsync(Suppiler suppiler)
        {
            try
            {
                if(suppiler is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();

                    dbContext.Suppilers.Update(suppiler);
                    await dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "SuppilersRepository", "UpdateLocationAsync", e);
                return false;
            }
        }
    }
}
