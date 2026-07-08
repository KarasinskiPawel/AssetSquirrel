using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentAssignment.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using System.Collections.Generic;
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
    }
}
