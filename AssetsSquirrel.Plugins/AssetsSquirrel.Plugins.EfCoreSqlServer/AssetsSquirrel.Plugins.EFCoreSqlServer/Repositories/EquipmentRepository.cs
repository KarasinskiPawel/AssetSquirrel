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
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly IDbContextFactory<AssetsSquirrelContext> dbContextFactory;
        private readonly IErrorsRepository errorsRepository;

        public EquipmentRepository(IDbContextFactory<AssetsSquirrelContext> dbContextFactory, IErrorsRepository errorsRepository)
        {
            this.dbContextFactory = dbContextFactory;
            this.errorsRepository = errorsRepository;
        }

        public async Task<bool> AddEquipmentAsync(Equipment equipment)
        {
            try
            {
                if(equipment is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();

                    dbContext.Equipments.Add(equipment);
                    dbContext.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentRepository", "AddEquipmentAsync", e);
                return false;
            }
        }

        public async Task<bool> DeleteEquipmentAsync(Equipment equipment)
        {
            try
            {
                if (equipment is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();

                    dbContext.Equipments.Remove(equipment);
                    await dbContext.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentRepository", "DeleteEquipmentAsync", e);
                return false;
            }
        }

        public async Task<List<EquipmentDto>> GetEquipmentAsync(Expression<Func<Equipment, bool>> where)
        {
            var dbContext = dbContextFactory.CreateDbContext();

            var output = await dbContext.Equipments.Where(where)
                .Select(a => new EquipmentDto
                {
                    EquipmentId = a.EquipmentId,
                    SuppilerId = a.SuppilerId,
                    SuppilerName = a.Suppiler.Name,
                    ManufacturerId = a.ManufacturerId,
                    ManufacturerName = a.Manufacturer.Name,
                    HardwareTypeId = a.HardwareTypeId,
                    HardwareTypeName = a.HardwareType.Name,
                    ModelName = a.ModelName,
                    IsActive = a.IsActive,
                    DateAdd = a.DateAdd,
                    DateRemoved = a.DateRemoved,
                    Description = a.Description,
                    InvoiceId = a.InvoiceId,
                    InvoiceNumber = a.Invoice != null ? a.Invoice.InvoiceNumber : null,
                    SerialNumber = a.SerialNumber,
                    UserId = a.UserId,
                    UserName = a.User != null ? a.User.UserName : null
                })
                .OrderBy(a => a.ModelName)
                .ThenBy(a => a.SerialNumber)
                .ToListAsync();

            return output;
        }

        public async Task<bool> UpdateEquipmentAsync(Equipment equipment)
        {
            try
            {
                if(equipment is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();

                    dbContext.Equipments.Update(equipment);
                    dbContext.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentRepository", "UpdateEquipmentAsync", e);
                return false;
            }
        }
    }
}
