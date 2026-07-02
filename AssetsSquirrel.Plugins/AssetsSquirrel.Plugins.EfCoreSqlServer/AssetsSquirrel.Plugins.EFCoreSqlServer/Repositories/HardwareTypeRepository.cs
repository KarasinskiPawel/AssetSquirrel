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
    public class HardwareTypeRepository : IHardwareTypeRepository
    {
        private readonly IDbContextFactory<AssetsSquirrelContext> dbContextFactory;
        private readonly IErrorsRepository errorsRepository;

        public HardwareTypeRepository(IDbContextFactory<AssetsSquirrelContext> dbContextFactory, IErrorsRepository errorsRepository)
        {
            this.dbContextFactory = dbContextFactory;
            this.errorsRepository = errorsRepository;
        }

        public async Task<Result<HardwareType>> AddHardwareTypeAsync(HardwareType hardwareType)
        {
            try
            {
                if (hardwareType is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();
                    dbContext.HardwareTypes.Add(hardwareType);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    return Result<HardwareType>.Fail("HardwareType cannot be null.");
                }
            }
            catch(Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "HardwareTypeRepository", "AddHardwareTypeAsync", e);
                return Result<HardwareType>.Fail(e.Message);
            }
            return Result<HardwareType>.Ok(hardwareType);
        }

        public async Task<Result<HardwareType>> DeleteHardwareTypeAsync(HardwareType hardwareType)
        {
            try
            {
                var dbContext = dbContextFactory.CreateDbContext();

                if (hardwareType is not null)
                {
                    dbContext.HardwareTypes.Remove(hardwareType);
                    dbContext.SaveChanges();
                    return Result<HardwareType>.Ok(hardwareType);
                }
                else
                {
                    return Result<HardwareType>.Fail("HardwareType cannot be null.");
                }
            }
            catch(Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "HardwareTypeRepository", "DeleteHardwareTypeAsync", e);
                return Result<HardwareType>.Fail(e.Message);
            }
        }

        public async Task<IEnumerable<HardwareType>> GetHardwareTypesAsync(Expression<Func<HardwareType, bool>> where)
        {
            var dbContext = dbContextFactory.CreateDbContext();

            var output = await dbContext.HardwareTypes
                .Where(where)
                .OrderBy(a => a.Name)
                .ToListAsync() ?? Enumerable.Empty<HardwareType>();

            return output;
        }

        public async Task<Result<HardwareType>> UpdateHardwareTypeAsync(HardwareType hardwareType)
        {
            try
            {
                if (hardwareType is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();

                    dbContext.HardwareTypes.Update(hardwareType);
                    dbContext.SaveChanges();
                    return Result<HardwareType>.Ok(hardwareType);
                }
                else
                {
                    return Result<HardwareType>.Fail("HardwareType cannot be null.");
                }
            }
            catch(Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "HardwareTypeRepository", "UpdateHardwareTypeAsync", e);
                return Result<HardwareType>.Fail(e.Message);
            }
        }
    }
}
