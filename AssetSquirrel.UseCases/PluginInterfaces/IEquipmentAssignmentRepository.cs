using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    }
}
