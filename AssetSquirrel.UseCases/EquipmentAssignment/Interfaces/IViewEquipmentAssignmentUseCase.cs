using AssetSquirrel.CoreBusiness.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.EquipmentAssignment.Interfaces
{
    public interface IViewEquipmentAssignmentUseCase
    {
        Task<List<EquipmentAssignmentOverviewDto>> GetEquipmentAssignmentOverviewAsync(EquipmentAssignmentFilter filter);
        Task<int> GetEquipmentAssignmentOverviewCountAsync(EquipmentAssignmentFilter filter, CancellationToken cancellationToken = default);
        Task<List<EquipmentAssignmentOverviewDto>> GetEquipmentAssignmentOverviewPageAsync(EquipmentAssignmentFilter filter, int startIndex, int count, string? sortColumn = null, bool sortDescending = false, CancellationToken cancellationToken = default);
    }
}
