using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentAssignment;
using AssetSquirrel.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
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

        public async Task<int> GetEquipmentAssignmentOverviewCountAsync(EquipmentAssignmentFilter filter, CancellationToken cancellationToken = default)
        {
            try
            {
                var dbContext = dbContextFactory.CreateDbContext();

                var joined =
                    from e in dbContext.Equipments
                    join a in dbContext.EquipmentAssignments.Where(x => x.DateOfReturn == null)
                        on e.EquipmentId equals a.EquipmentId into assignmentGroup
                    from assignment in assignmentGroup.DefaultIfEmpty()
                    select new { Equipment = e, Assignment = assignment };

                joined = joined.Where(x => x.Equipment.IsActive == filter.IsActive);

                if (filter.LocationId is not null)
                {
                    joined = joined.Where(x => x.Assignment != null && x.Assignment.LocationId == filter.LocationId);
                }

                if (filter.EmployeeId is not null)
                {
                    joined = joined.Where(x => x.Assignment != null && x.Assignment.EmployeeId == filter.EmployeeId);
                }

                if (filter.ManufacturerId is not null)
                {
                    joined = joined.Where(x => x.Equipment.ManufacturerId == filter.ManufacturerId);
                }

                if (filter.HardwareTypeId is not null)
                {
                    joined = joined.Where(x => x.Equipment.HardwareTypeId == filter.HardwareTypeId);
                }

                if (!string.IsNullOrEmpty(filter.SearchText))
                {
                    joined = joined.Where(x =>
                        (x.Equipment.SerialNumber != null && x.Equipment.SerialNumber.Contains(filter.SearchText)) ||
                        (x.Equipment.InventoryNumber != null && x.Equipment.InventoryNumber.Contains(filter.SearchText)) ||
                        (x.Equipment.Invoice != null && x.Equipment.Invoice.InvoiceNumber != null && x.Equipment.Invoice.InvoiceNumber.Contains(filter.SearchText)));
                }

                return await joined.CountAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentAssignmentRepository", "GetEquipmentAssignmentOverviewCountAsync", e);
                return 0;
            }
        }

        public async Task<List<EquipmentAssignmentOverviewDto>> GetEquipmentAssignmentOverviewPageAsync(EquipmentAssignmentFilter filter, int startIndex, int count, string sortColumn = null, bool sortDescending = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var dbContext = dbContextFactory.CreateDbContext();

                var joined =
                    from e in dbContext.Equipments
                    join a in dbContext.EquipmentAssignments.Where(x => x.DateOfReturn == null)
                        on e.EquipmentId equals a.EquipmentId into assignmentGroup
                    from assignment in assignmentGroup.DefaultIfEmpty()
                    select new { Equipment = e, Assignment = assignment };

                joined = joined.Where(x => x.Equipment.IsActive == filter.IsActive);

                if (filter.LocationId is not null)
                {
                    joined = joined.Where(x => x.Assignment != null && x.Assignment.LocationId == filter.LocationId);
                }

                if (filter.EmployeeId is not null)
                {
                    joined = joined.Where(x => x.Assignment != null && x.Assignment.EmployeeId == filter.EmployeeId);
                }

                if (filter.ManufacturerId is not null)
                {
                    joined = joined.Where(x => x.Equipment.ManufacturerId == filter.ManufacturerId);
                }

                if (filter.HardwareTypeId is not null)
                {
                    joined = joined.Where(x => x.Equipment.HardwareTypeId == filter.HardwareTypeId);
                }

                if (!string.IsNullOrEmpty(filter.SearchText))
                {
                    joined = joined.Where(x =>
                        (x.Equipment.SerialNumber != null && x.Equipment.SerialNumber.Contains(filter.SearchText)) ||
                        (x.Equipment.InventoryNumber != null && x.Equipment.InventoryNumber.Contains(filter.SearchText)) ||
                        (x.Equipment.Invoice != null && x.Equipment.Invoice.InvoiceNumber != null && x.Equipment.Invoice.InvoiceNumber.Contains(filter.SearchText)));
                }

                var ordered = sortColumn switch
                {
                    nameof(EquipmentAssignmentOverviewDto.AssignedLocationName) => sortDescending
                        ? joined.OrderByDescending(x => x.Assignment.Location.City).ThenByDescending(x => x.Assignment.Location.Street)
                        : joined.OrderBy(x => x.Assignment.Location.City).ThenBy(x => x.Assignment.Location.Street),
                    nameof(EquipmentAssignmentOverviewDto.AssignedEmployeeName) => sortDescending
                        ? joined.OrderByDescending(x => x.Assignment.Employee.FirstName).ThenByDescending(x => x.Assignment.Employee.LastName)
                        : joined.OrderBy(x => x.Assignment.Employee.FirstName).ThenBy(x => x.Assignment.Employee.LastName),
                    _ => joined.OrderBy(x => x.Equipment.EquipmentId)
                };

                return await ordered
                    .Skip(startIndex)
                    .Take(count)
                    .Select(x => new EquipmentAssignmentOverviewDto
                    {
                        EquipmentId = x.Equipment.EquipmentId,
                        SuppilerName = x.Equipment.Suppiler != null ? x.Equipment.Suppiler.Name : null,
                        ManufacturerId = x.Equipment.ManufacturerId,
                        ManufacturerName = x.Equipment.Manufacturer != null ? x.Equipment.Manufacturer.Name : null,
                        HardwareTypeId = x.Equipment.HardwareTypeId,
                        HardwareTypeName = x.Equipment.HardwareType != null ? x.Equipment.HardwareType.Name : null,
                        ModelName = x.Equipment.ModelName,
                        SerialNumber = x.Equipment.SerialNumber,
                        InventoryNumber = x.Equipment.InventoryNumber,
                        InvoiceNumber = x.Equipment.Invoice != null ? x.Equipment.Invoice.InvoiceNumber : null,
                        IsActive = x.Equipment.IsActive,
                        AssignedEmployeeId = x.Assignment != null ? x.Assignment.EmployeeId : null,
                        AssignedEmployeeName = x.Assignment != null && x.Assignment.Employee != null
                            ? x.Assignment.Employee.FirstName + " " + x.Assignment.Employee.LastName
                            : null,
                        AssignedLocationId = x.Assignment != null ? x.Assignment.LocationId : null,
                        AssignedLocationName = x.Assignment != null && x.Assignment.Location != null
                            ? x.Assignment.Location.City + " " + x.Assignment.Location.Street
                            : null,
                        DateOfHandover = x.Assignment != null ? x.Assignment.DateOfHandover : null
                    })
                    .ToListAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Virtualize cancels a superseded ItemsProviderRequest (e.g. during
                // initial viewport measurement or fast scrolling) via its own token --
                // this is expected control flow, not an error. Let it propagate so
                // Virtualize's own cancellation handling deals with it; logging it
                // here would flood the Errors table and mask real failures.
                throw;
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentAssignmentRepository", "GetEquipmentAssignmentOverviewPageAsync", e);
                return new List<EquipmentAssignmentOverviewDto>();
            }
        }
    }
}
