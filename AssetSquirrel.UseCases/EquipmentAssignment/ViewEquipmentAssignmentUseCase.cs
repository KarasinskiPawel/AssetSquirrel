using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentAssignment.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.EquipmentAssignment
{
    public class ViewEquipmentAssignmentUseCase : IViewEquipmentAssignmentUseCase
    {
        private readonly IEquipmentAssignmentRepository equipmentAssignmentRepository;

        public ViewEquipmentAssignmentUseCase(IEquipmentAssignmentRepository equipmentAssignmentRepository)
        {
            this.equipmentAssignmentRepository = equipmentAssignmentRepository;
        }

        public async Task<List<EquipmentAssignmentOverviewDto>> GetEquipmentAssignmentOverviewAsync(EquipmentAssignmentFilter filter)
        {
            return await equipmentAssignmentRepository.GetEquipmentAssignmentOverviewAsync(filter);
        }

        public async Task<int> GetEquipmentAssignmentOverviewCountAsync(EquipmentAssignmentFilter filter, CancellationToken cancellationToken = default)
        {
            return await equipmentAssignmentRepository.GetEquipmentAssignmentOverviewCountAsync(filter, cancellationToken);
        }

        public async Task<List<EquipmentAssignmentOverviewDto>> GetEquipmentAssignmentOverviewPageAsync(EquipmentAssignmentFilter filter, int startIndex, int count, string? sortColumn = null, bool sortDescending = false, CancellationToken cancellationToken = default)
        {
            return await equipmentAssignmentRepository.GetEquipmentAssignmentOverviewPageAsync(filter, startIndex, count, sortColumn, sortDescending, cancellationToken);
        }
    }
}
