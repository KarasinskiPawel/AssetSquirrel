using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.PluginInterfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories
{
    public class EquipmentReturnRepository : IEquipmentReturnRepository
    {
        private readonly IDbContextFactory<AssetsSquirrelContext> contextFactory;
        private readonly IErrorsRepository errorsRepository;

        public EquipmentReturnRepository(IDbContextFactory<AssetsSquirrelContext> contextFactory, IErrorsRepository errorsRepository)
        {
            this.contextFactory = contextFactory;
            this.errorsRepository = errorsRepository;
        }

        public async Task<IEnumerable<EquipmentReturn>> GetEquipmentReturnsAsync(Expression<Func<EquipmentReturn, bool>> where)
        {
            try
            {
                var dbContext = contextFactory.CreateDbContext();
                return await dbContext.EquipmentReturns
                    .Include(r => r.Employee)
                    .Include(r => r.Location)
                    .Include(r => r.StorageLocation)
                    .Include(r => r.PreparedByUser)
                    .Include(r => r.EquipmentAssignments).ThenInclude(a => a.Equipment).ThenInclude(e => e.Manufacturer)
                    .Include(r => r.EquipmentAssignments).ThenInclude(a => a.Equipment).ThenInclude(e => e.HardwareType)
                    .Where(where)
                    .ToListAsync() ?? Enumerable.Empty<EquipmentReturn>();
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentReturnRepository", "GetEquipmentReturnsAsync", e);
                return Enumerable.Empty<EquipmentReturn>();
            }
        }

        public async Task<Result<EquipmentReturn>> PostEquipmentReturnAsync(EquipmentReturn equipmentReturn, List<int> equipmentAssignmentIds, string preparedByUserId)
        {
            try
            {
                if (equipmentReturn is null)
                {
                    return Result<EquipmentReturn>.Fail("EquipmentReturn cannot be null.");
                }

                equipmentReturn.PreparedByUserId = preparedByUserId;

                const int maxAttempts = 3;

                for (var attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    var dbContext = contextFactory.CreateDbContext();

                    var assignments = await dbContext.EquipmentAssignments
                        .Include(a => a.Equipment)
                        .Where(a => equipmentAssignmentIds.Contains(a.EquipmentAssignmentId) && a.DateOfReturn == null)
                        .ToListAsync();

                    if (assignments.Count != equipmentAssignmentIds.Distinct().Count())
                    {
                        return Result<EquipmentReturn>.Fail("One or more selected items have already been returned. Please refresh and try again.");
                    }

                    equipmentReturn.ReturnDocumentNumber = await GenerateNextDocumentNumberAsync(dbContext, equipmentReturn.ReturnDate);

                    dbContext.EquipmentReturns.Add(equipmentReturn);

                    foreach (var assignment in assignments)
                    {
                        assignment.DateOfReturn = equipmentReturn.ReturnDate;
                        assignment.EquipmentReturn = equipmentReturn;

                        if (assignment.Equipment is not null)
                        {
                            assignment.Equipment.LocationId = equipmentReturn.StorageLocationId;
                        }

                        dbContext.EquipmentAssignmentHistories.Add(new EquipmentAssignmentHistory
                        {
                            EquipmentAssignment = assignment,
                            EquipmentId = assignment.EquipmentId,
                            LocationId = assignment.LocationId,
                            EmployeeId = assignment.EmployeeId,
                            DateOfHandover = assignment.DateOfHandover,
                            DateOfReturn = assignment.DateOfReturn,
                            UserId = preparedByUserId
                        });
                    }

                    var affectedHandoverIds = assignments
                        .Where(a => a.EquipmentHandoverId.HasValue)
                        .Select(a => a.EquipmentHandoverId!.Value)
                        .Distinct()
                        .ToList();

                    if (affectedHandoverIds.Count > 0)
                    {
                        var stillOpenHandoverIds = await dbContext.EquipmentAssignments
                            .Where(a => a.EquipmentHandoverId.HasValue
                                && affectedHandoverIds.Contains(a.EquipmentHandoverId.Value)
                                && a.DateOfReturn == null
                                && !equipmentAssignmentIds.Contains(a.EquipmentAssignmentId))
                            .Select(a => a.EquipmentHandoverId!.Value)
                            .Distinct()
                            .ToListAsync();

                        var handoverIdsToClose = affectedHandoverIds.Except(stillOpenHandoverIds).ToList();

                        if (handoverIdsToClose.Count > 0)
                        {
                            var handoversToClose = await dbContext.EquipmentHandovers
                                .Where(h => handoverIdsToClose.Contains(h.EquipmentHandoverId))
                                .ToListAsync();

                            foreach (var handover in handoversToClose)
                            {
                                handover.IsActive = false;
                            }
                        }
                    }

                    try
                    {
                        await dbContext.SaveChangesAsync();
                        return Result<EquipmentReturn>.Ok(equipmentReturn);
                    }
                    catch (DbUpdateException e) when (attempt < maxAttempts && IsDuplicateKeyViolation(e, "ReturnDocumentNumber"))
                    {
                        // Two saves generated the same document number -- retry
                        // with a freshly computed one.
                        continue;
                    }
                    catch (DbUpdateException e)
                    {
                        await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentReturnRepository", "PostEquipmentReturnAsync", e);
                        return Result<EquipmentReturn>.Fail(e.InnerException?.Message ?? e.Message);
                    }
                }

                return Result<EquipmentReturn>.Fail("Could not save the return document after multiple attempts. Please try again.");
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentReturnRepository", "PostEquipmentReturnAsync", e);
                return Result<EquipmentReturn>.Fail(e.Message);
            }
        }

        public async Task<Result<EquipmentReturn>> UpdateEquipmentReturnAsync(EquipmentReturn equipmentReturn)
        {
            try
            {
                if (equipmentReturn is null)
                {
                    return Result<EquipmentReturn>.Fail("EquipmentReturn cannot be null.");
                }

                var dbContext = contextFactory.CreateDbContext();
                dbContext.Update(equipmentReturn);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentReturnRepository", "UpdateEquipmentReturnAsync", e);
                return Result<EquipmentReturn>.Fail(e.Message);
            }

            return Result<EquipmentReturn>.Ok(equipmentReturn);
        }

        private static async Task<string> GenerateNextDocumentNumberAsync(AssetsSquirrelContext dbContext, DateTime returnDate)
        {
            var prefix = $"{returnDate:yyyy}/{returnDate:MM}/";

            var lastNumber = await dbContext.EquipmentReturns
                .Where(r => r.ReturnDocumentNumber.StartsWith(prefix))
                .OrderByDescending(r => r.ReturnDocumentNumber)
                .Select(r => r.ReturnDocumentNumber)
                .FirstOrDefaultAsync();

            var nextSequence = 1;
            if (lastNumber is not null && int.TryParse(lastNumber.Substring(prefix.Length), out var parsedSequence))
            {
                nextSequence = parsedSequence + 1;
            }

            return $"{prefix}{nextSequence:D4}";
        }

        private static bool IsDuplicateKeyViolation(DbUpdateException exception, string indexNameHint)
        {
            return exception.InnerException is SqlException sqlException
                && (sqlException.Number == 2601 || sqlException.Number == 2627)
                && sqlException.Message.Contains(indexNameHint, StringComparison.OrdinalIgnoreCase);
        }
    }
}
