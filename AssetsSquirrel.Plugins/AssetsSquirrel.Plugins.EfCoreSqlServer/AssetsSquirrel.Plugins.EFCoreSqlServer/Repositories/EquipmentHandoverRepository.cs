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
    public class EquipmentHandoverRepository : IEquipmentHandoverRepository
    {
        private readonly IDbContextFactory<AssetsSquirrelContext> contextFactory;
        private readonly IErrorsRepository errorsRepository;

        public EquipmentHandoverRepository(IDbContextFactory<AssetsSquirrelContext> contextFactory, IErrorsRepository errorsRepository)
        {
            this.contextFactory = contextFactory;
            this.errorsRepository = errorsRepository;
        }

        public async Task<IEnumerable<EquipmentHandover>> GetEquipmentHandoversAsync(Expression<Func<EquipmentHandover, bool>> where)
        {
            try
            {
                var dbContext = contextFactory.CreateDbContext();
                return await dbContext.EquipmentHandovers.Where(where)
                    .ToListAsync() ?? Enumerable.Empty<EquipmentHandover>();
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentHandoverRepository", "GetEquipmentHandoversAsync", e);
                return Enumerable.Empty<EquipmentHandover>();
            }
        }

        public async Task<bool> AddEquipmentHandoverAsync(EquipmentHandover equipmentHandover)
        {
            try
            {
                if (equipmentHandover is not null)
                {
                    var dbContext = contextFactory.CreateDbContext();
                    dbContext.EquipmentHandovers.Add(equipmentHandover);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentHandoverRepository", "AddEquipmentHandoverAsync", e);
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateEquipmentHandoverAsync(EquipmentHandover equipmentHandover)
        {
            try
            {
                if(equipmentHandover is not null)
                {
                    var dbContext = contextFactory.CreateDbContext();
                    dbContext.Update(equipmentHandover);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentHandoverRepository", "UpdateEquipmentHandover", e);
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteEquipmentHandoverAsync(EquipmentHandover equipmentHandover)
        {
            try
            {
                var dbContext = contextFactory.CreateDbContext();
                dbContext.Remove(equipmentHandover);
                await dbContext.SaveChangesAsync();
            }
            catch(Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentHandoverRepository", "DeleteEquipmentHandoverAsync", e);
                return false;
            }

            return true;
        }
    }
}
