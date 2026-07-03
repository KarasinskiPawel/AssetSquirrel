using AssetSquirrel.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories
{
    public class EquipmentAssignmentRepository : IEquipmentAssignmentRepository
    {
        private readonly IDbContextFactory<AssetsSquirrelContext> dbContextFactory;

        public EquipmentAssignmentRepository(IDbContextFactory<AssetsSquirrelContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public async Task<List<int>> GetAssignedEquipmentIdsAsync()
        {
            var dbContext = dbContextFactory.CreateDbContext();

            return await dbContext.EquipmentAssignments
                .Where(a => a.DateOfReturn == null)
                .Select(a => a.EquipmentId)
                .ToListAsync();
        }
    }
}
