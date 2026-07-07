using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentReturn.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Mapster;

namespace AssetSquirrel.UseCases.EquipmentReturn
{
    public class EditEquipmentReturnUseCase : IEditEquipmentReturnUseCase
    {
        private readonly IEquipmentReturnRepository equipmentReturnRepository;

        public EditEquipmentReturnUseCase(IEquipmentReturnRepository equipmentReturnRepository)
        {
            this.equipmentReturnRepository = equipmentReturnRepository;
        }

        public async Task<Result<EquipmentReturnDto>> UpdateEquipmentReturnAsync(EquipmentReturnDto equipmentReturn)
        {
            var result = await equipmentReturnRepository.UpdateEquipmentReturnAsync(equipmentReturn.Adapt<AssetSquirrel.CoreBusiness.EquipmentReturn>());

            return result.Select(r => r.Adapt<EquipmentReturnDto>());
        }
    }
}
