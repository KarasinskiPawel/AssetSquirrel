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

        public async Task<Result<Equipment>> AddEquipmentAsync(Equipment equipment)
        {
            try
            {
                if(equipment is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();

                    dbContext.Equipments.Add(equipment);

                    var history = BuildHistorySnapshot(equipment);
                    history.Equipment = equipment; // lets EF fix up EquipmentId once the new Equipment's identity is generated
                    dbContext.EquipmentHistories.Add(history);

                    await dbContext.SaveChangesAsync();

                    return Result<Equipment>.Ok(equipment);
                }
                else
                {
                    return Result<Equipment>.Fail("Equipment cannot be null.");
                }
            }
            catch(Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentRepository", "AddEquipmentAsync", e);
                return Result<Equipment>.Fail(e.Message);
            }
        }

        public async Task<Result<Equipment>> DeleteEquipmentAsync(Equipment equipment)
        {
            try
            {
                if (equipment is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();

                    // NOTE: EquipmentHistoryConfiguration cascades deletes from Equipment to
                    // EquipmentHistories, so any history row tied to this EquipmentId --
                    // including the snapshot below -- is removed by the database the instant
                    // this Equipment row is deleted. A durable "deleted" audit entry is not
                    // achievable under the current schema without changing that cascade
                    // behavior. Equipment is not hard-deleted anywhere in the app today
                    // (Equipment.razor deactivates via IsActive instead) -- this history
                    // write is best-effort, kept for interface completeness only.
                    var history = BuildHistorySnapshot(equipment);
                    history.EquipmentId = equipment.EquipmentId;
                    dbContext.EquipmentHistories.Add(history);

                    dbContext.Equipments.Remove(equipment);
                    await dbContext.SaveChangesAsync();

                    return Result<Equipment>.Ok(equipment);
                }
                else
                {
                    return Result<Equipment>.Fail("Equipment cannot be null.");
                }
            }
            catch(Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentRepository", "DeleteEquipmentAsync", e);
                return Result<Equipment>.Fail(e.Message);
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
                    RegisteredByUserId = a.RegisteredByUserId,
                    RegisteredByUserName = a.RegisteredByUser != null ? a.RegisteredByUser.UserName : null
                })
                .OrderBy(a => a.ModelName)
                .ThenBy(a => a.SerialNumber)
                .ToListAsync();

            return output;
        }

        public async Task<Result<Equipment>> UpdateEquipmentAsync(Equipment equipment)
        {
            try
            {
                if(equipment is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();

                    dbContext.Equipments.Update(equipment);

                    var history = BuildHistorySnapshot(equipment);
                    history.EquipmentId = equipment.EquipmentId;
                    dbContext.EquipmentHistories.Add(history);

                    await dbContext.SaveChangesAsync();

                    return Result<Equipment>.Ok(equipment);
                }
                else
                {
                    return Result<Equipment>.Fail("Equipment cannot be null.");
                }
            }
            catch(Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentRepository", "UpdateEquipmentAsync", e);
                return Result<Equipment>.Fail(e.Message);
            }
        }

        // Snapshot of an Equipment row written to EquipmentHistories on every
        // Add/Update/Delete so changes to the table are auditable over time.
        private static EquipmentHistory BuildHistorySnapshot(Equipment equipment)
        {
            return new EquipmentHistory
            {
                SuppilerId = equipment.SuppilerId,
                ManufacturerId = equipment.ManufacturerId,
                HardwareTypeId = equipment.HardwareTypeId,
                InvoiceId = equipment.InvoiceId,
                ModelName = equipment.ModelName,
                SerialNumber = equipment.SerialNumber,
                Description = equipment.Description,
                DateAdd = equipment.DateAdd,
                DateRemoved = equipment.DateRemoved,
                IsActive = equipment.IsActive,
                ApplicationUserId = equipment.RegisteredByUserId
            };
        }
    }
}
