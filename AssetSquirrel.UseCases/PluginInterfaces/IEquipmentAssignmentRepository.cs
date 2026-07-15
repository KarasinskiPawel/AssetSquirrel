using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IEquipmentAssignmentRepository
    {
        Task<List<int>> GetAssignedEquipmentIdsAsync();
        // Fully qualified: "AssetSquirrel.UseCases.EquipmentAssignment" (the feature
        // namespace) shadows the CoreBusiness.EquipmentAssignment entity type here.
        Task<IEnumerable<CoreBusiness.EquipmentAssignment>> GetOpenAssignmentsAsync(Expression<Func<CoreBusiness.EquipmentAssignment, bool>> where);
        Task<List<EquipmentAssignmentOverviewDto>> GetEquipmentAssignmentOverviewAsync(AssetSquirrel.UseCases.EquipmentAssignment.EquipmentAssignmentFilter filter);

        // Count and page are separate so a Virtualize ItemsProvider can fetch the
        // count once per filter change and reuse it across every scroll-triggered
        // page fetch, instead of recomputing it (a LEFT JOIN query on its own) on
        // every single batch.
        Task<int> GetEquipmentAssignmentOverviewCountAsync(AssetSquirrel.UseCases.EquipmentAssignment.EquipmentAssignmentFilter filter, CancellationToken cancellationToken = default);
        Task<List<EquipmentAssignmentOverviewDto>> GetEquipmentAssignmentOverviewPageAsync(AssetSquirrel.UseCases.EquipmentAssignment.EquipmentAssignmentFilter filter, int startIndex, int count, string? sortColumn = null, bool sortDescending = false, CancellationToken cancellationToken = default);
    }
}
