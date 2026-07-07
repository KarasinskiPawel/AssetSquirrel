using AssetSquirrel.CoreBusiness;
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
    }
}
