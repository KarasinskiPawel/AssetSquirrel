using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.PluginInterfaces;
using Microsoft.Data.SqlClient;
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
                return await dbContext.EquipmentHandovers
                    .Include(h => h.ToLocation)
                    .Include(h => h.ToEmployee)
                    .Include(h => h.PreparedByUser)
                    .Include(h => h.EquipmentHandoverDetails).ThenInclude(d => d.Equipment).ThenInclude(e => e.Manufacturer)
                    .Include(h => h.EquipmentHandoverDetails).ThenInclude(d => d.Equipment).ThenInclude(e => e.HardwareType)
                    .Where(where)
                    .ToListAsync() ?? Enumerable.Empty<EquipmentHandover>();
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentHandoverRepository", "GetEquipmentHandoversAsync", e);
                return Enumerable.Empty<EquipmentHandover>();
            }
        }

        public async Task<Result<EquipmentHandover>> AddEquipmentHandoverAsync(EquipmentHandover equipmentHandover)
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
                    return Result<EquipmentHandover>.Fail("EquipmentHandover cannot be null.");
                }
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentHandoverRepository", "AddEquipmentHandoverAsync", e);
                return Result<EquipmentHandover>.Fail(e.Message);
            }
            return Result<EquipmentHandover>.Ok(equipmentHandover);
        }

        public async Task<Result<EquipmentHandover>> UpdateEquipmentHandoverAsync(EquipmentHandover equipmentHandover)
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
                    return Result<EquipmentHandover>.Fail("EquipmentHandover cannot be null.");
                }
            }
            catch(Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentHandoverRepository", "UpdateEquipmentHandover", e);
                return Result<EquipmentHandover>.Fail(e.Message);
            }

            return Result<EquipmentHandover>.Ok(equipmentHandover);
        }

        public async Task<Result<EquipmentHandover>> DeleteEquipmentHandoverAsync(EquipmentHandover equipmentHandover)
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
                return Result<EquipmentHandover>.Fail(e.Message);
            }

            return Result<EquipmentHandover>.Ok(equipmentHandover);
        }

        public async Task<Result<EquipmentHandover>> PostEquipmentHandoverAsync(EquipmentHandover equipmentHandover, string preparedByUserId)
        {
            try
            {
                if (equipmentHandover is null)
                {
                    return Result<EquipmentHandover>.Fail("EquipmentHandover cannot be null.");
                }

                equipmentHandover.PreparedByUserId = preparedByUserId;

                const int maxAttempts = 3;

                for (var attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    var dbContext = contextFactory.CreateDbContext();

                    equipmentHandover.HandoverDocumentNumber = await GenerateNextDocumentNumberAsync(dbContext, equipmentHandover.HandoverDate);

                    dbContext.EquipmentHandovers.Add(equipmentHandover);

                    foreach (var detail in equipmentHandover.EquipmentHandoverDetails)
                    {
                        var assignment = new EquipmentAssignment
                        {
                            EquipmentId = detail.EquipmentId,
                            LocationId = equipmentHandover.ToLocationId,
                            EmployeeId = equipmentHandover.ToEmployeeId,
                            DateOfHandover = equipmentHandover.HandoverDate,
                            DateOfReturn = null,
                            UserId = preparedByUserId,
                            // Lets EF fix up EquipmentHandoverId once the new
                            // handover's identity is generated (same pattern as
                            // EquipmentRepository's history-snapshot fixup).
                            EquipmentHandover = equipmentHandover
                        };
                        dbContext.EquipmentAssignments.Add(assignment);

                        dbContext.EquipmentAssignmentHistories.Add(new EquipmentAssignmentHistory
                        {
                            EquipmentAssignment = assignment,
                            EquipmentId = assignment.EquipmentId,
                            LocationId = assignment.LocationId,
                            EmployeeId = assignment.EmployeeId,
                            DateOfHandover = assignment.DateOfHandover,
                            DateOfReturn = assignment.DateOfReturn,
                            UserId = assignment.UserId
                        });
                    }

                    try
                    {
                        await dbContext.SaveChangesAsync();
                        return Result<EquipmentHandover>.Ok(equipmentHandover);
                    }
                    catch (DbUpdateException e) when (attempt < maxAttempts && IsDuplicateKeyViolation(e, "HandoverDocumentNumber"))
                    {
                        // Two saves generated the same document number -- retry
                        // with a freshly computed one.
                        continue;
                    }
                    catch (DbUpdateException e) when (IsDuplicateKeyViolation(e, "EquipmentId"))
                    {
                        await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentHandoverRepository", "PostEquipmentHandoverAsync", e);
                        return Result<EquipmentHandover>.Fail("One or more items have already been handed over to someone else. Please refresh and try again.");
                    }
                    catch (DbUpdateException e)
                    {
                        await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentHandoverRepository", "PostEquipmentHandoverAsync", e);
                        return Result<EquipmentHandover>.Fail(e.InnerException?.Message ?? e.Message);
                    }
                }

                return Result<EquipmentHandover>.Fail("Could not save the handover document after multiple attempts. Please try again.");
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentHandoverRepository", "PostEquipmentHandoverAsync", e);
                return Result<EquipmentHandover>.Fail(e.Message);
            }
        }

        public async Task<Result<EquipmentHandover>> CancelEquipmentHandoverAsync(int equipmentHandoverId, string cancelledByUserId)
        {
            try
            {
                var dbContext = contextFactory.CreateDbContext();

                var handover = await dbContext.EquipmentHandovers
                    .FirstOrDefaultAsync(h => h.EquipmentHandoverId == equipmentHandoverId);

                if (handover is null)
                {
                    return Result<EquipmentHandover>.Fail("Equipment handover not found.");
                }

                handover.IsActive = false;

                var openAssignments = await dbContext.EquipmentAssignments
                    .Where(a => a.EquipmentHandoverId == equipmentHandoverId && a.DateOfReturn == null)
                    .ToListAsync();

                foreach (var assignment in openAssignments)
                {
                    assignment.DateOfReturn = DateTime.Now;

                    dbContext.EquipmentAssignmentHistories.Add(new EquipmentAssignmentHistory
                    {
                        EquipmentAssignment = assignment,
                        EquipmentId = assignment.EquipmentId,
                        LocationId = assignment.LocationId,
                        EmployeeId = assignment.EmployeeId,
                        DateOfHandover = assignment.DateOfHandover,
                        DateOfReturn = assignment.DateOfReturn,
                        UserId = cancelledByUserId
                    });
                }

                await dbContext.SaveChangesAsync();

                return Result<EquipmentHandover>.Ok(handover);
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EquipmentHandoverRepository", "CancelEquipmentHandoverAsync", e);
                return Result<EquipmentHandover>.Fail(e.Message);
            }
        }

        private static async Task<string> GenerateNextDocumentNumberAsync(AssetsSquirrelContext dbContext, DateTime handoverDate)
        {
            var prefix = $"{handoverDate:yyyy}/{handoverDate:MM}/";

            var lastNumber = await dbContext.EquipmentHandovers
                .Where(h => h.HandoverDocumentNumber.StartsWith(prefix))
                .OrderByDescending(h => h.HandoverDocumentNumber)
                .Select(h => h.HandoverDocumentNumber)
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
