using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentHandover.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Mapster;

namespace AssetSquirrel.UseCases.EquipmentHandover
{
    public class EditEquipmentHandoverUseCase : IEditEquipmentHandoverUseCase
    {
        private readonly IEquipmentHandoverRepository equipmentHandoverRepository;

        public EditEquipmentHandoverUseCase(IEquipmentHandoverRepository equipmentHandoverRepository)
        {
            this.equipmentHandoverRepository = equipmentHandoverRepository;
        }

        public async Task<Result<EquipmentHandoverDto>> UpdateEquipmentHandoverAsync(EquipmentHandoverDto handover)
        {
            var result = await equipmentHandoverRepository.UpdateEquipmentHandoverAsync(handover.Adapt<AssetSquirrel.CoreBusiness.EquipmentHandover>());

            return result.Select(e => e.Adapt<EquipmentHandoverDto>());
        }

        public async Task<Result<EquipmentHandoverDto>> CancelEquipmentHandoverAsync(int equipmentHandoverId, string cancelledByUserId)
        {
            var result = await equipmentHandoverRepository.CancelEquipmentHandoverAsync(equipmentHandoverId, cancelledByUserId);

            return result.Select(e => e.Adapt<EquipmentHandoverDto>());
        }
    }
}
