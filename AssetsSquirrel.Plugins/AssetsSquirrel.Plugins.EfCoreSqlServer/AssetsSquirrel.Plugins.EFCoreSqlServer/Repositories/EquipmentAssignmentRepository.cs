using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentAssignment;
using AssetSquirrel.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories
{
    public class EquipmentAssignmentRepository : IEquipmentAssignmentRepository
    {
        private readonly IDbContextFactory<AssetsSquirrelContext> dbContextFactory;
        private readonly IErrorsRepository errorsRepository;

        public EquipmentAssignmentRepository(IDbContextFactory<AssetsSquirrelContext> dbContextFactory, IErrorsRepository errorsRepository)
        {
            this.dbContextFactory = dbContextFactory;
            this.errorsRepository = errorsRepository;
        }

        public async Task<List<int>> GetAssignedEquipmentIdsAsync()
        {
            try
            {
                var dbContext = dbContextFactory.CreateDbContext();

                return await dbContext.EquipmentAssignments
                    .Where(a => a.DateOfReturn == null)
                    .Select(a => a.EquipmentId)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentAssignmentRepository", "GetAssignedEquipmentIdsAsync", e);
                return new List<int>();
            }
        }

        public async Task<IEnumerable<EquipmentAssignment>> GetOpenAssignmentsAsync(Expression<Func<EquipmentAssignment, bool>> where)
        {
            try
            {
                var dbContext = dbContextFactory.CreateDbContext();

                return await dbContext.EquipmentAssignments
                    .Include(a => a.Equipment).ThenInclude(e => e.Manufacturer)
                    .Include(a => a.Equipment).ThenInclude(e => e.HardwareType)
                    .Include(a => a.Location)
                    .Include(a => a.Employee)
                    .Where(where)
                    .ToListAsync() ?? Enumerable.Empty<EquipmentAssignment>();
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentAssignmentRepository", "GetOpenAssignmentsAsync", e);
                return Enumerable.Empty<EquipmentAssignment>();
            }
        }

        public async Task<List<EquipmentAssignmentOverviewDto>> GetEquipmentAssignmentOverviewAsync(EquipmentAssignmentFilter filter)
        {
            try
            {
                var dbContext = dbContextFactory.CreateDbContext();

                var query =
                    from e in dbContext.Equipments
                    join a in dbContext.EquipmentAssignments.Where(x => x.DateOfReturn == null)
                        on e.EquipmentId equals a.EquipmentId into assignmentGroup
                    from assignment in assignmentGroup.DefaultIfEmpty()
                    select new EquipmentAssignmentOverviewDto
                    {
                        EquipmentId = e.EquipmentId,
                        SuppilerName = e.Suppiler != null ? e.Suppiler.Name : null,
                        ManufacturerId = e.ManufacturerId,
                        ManufacturerName = e.Manufacturer != null ? e.Manufacturer.Name : null,
                        HardwareTypeId = e.HardwareTypeId,
                        HardwareTypeName = e.HardwareType != null ? e.HardwareType.Name : null,
                        ModelName = e.ModelName,
                        SerialNumber = e.SerialNumber,
                        InventoryNumber = e.InventoryNumber,
                        InvoiceNumber = e.Invoice != null ? e.Invoice.InvoiceNumber : null,
                        IsActive = e.IsActive,
                        AssignedEmployeeId = assignment != null ? assignment.EmployeeId : null,
                        AssignedEmployeeName = assignment != null && assignment.Employee != null
                            ? assignment.Employee.FirstName + " " + assignment.Employee.LastName
                            : null,
                        AssignedLocationId = assignment != null ? assignment.LocationId : null,
                        AssignedLocationName = assignment != null && assignment.Location != null
                            ? assignment.Location.City + " " + assignment.Location.Street
                            : null,
                        DateOfHandover = assignment != null ? assignment.DateOfHandover : null
                    };

                query = query.Where(x => x.IsActive == filter.IsActive);

                if (filter.LocationId is not null)
                {
                    query = query.Where(x => x.AssignedLocationId == filter.LocationId);
                }

                if (filter.EmployeeId is not null)
                {
                    query = query.Where(x => x.AssignedEmployeeId == filter.EmployeeId);
                }

                if (filter.ManufacturerId is not null)
                {
                    query = query.Where(x => x.ManufacturerId == filter.ManufacturerId);
                }

                if (filter.HardwareTypeId is not null)
                {
                    query = query.Where(x => x.HardwareTypeId == filter.HardwareTypeId);
                }

                if (!string.IsNullOrEmpty(filter.SearchText))
                {
                    query = query.Where(x =>
                        (x.SerialNumber != null && x.SerialNumber.Contains(filter.SearchText)) ||
                        (x.InventoryNumber != null && x.InventoryNumber.Contains(filter.SearchText)) ||
                        (x.InvoiceNumber != null && x.InvoiceNumber.Contains(filter.SearchText)));
                }

                return await query.ToListAsync();
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentAssignmentRepository", "GetEquipmentAssignmentOverviewAsync", e);
                return new List<EquipmentAssignmentOverviewDto>();
            }
        }
    }
}
