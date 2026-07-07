using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.EquipmentReturn.Interfaces
{
    public interface IEditEquipmentReturnUseCase
    {
        Task<Result<EquipmentReturnDto>> UpdateEquipmentReturnAsync(EquipmentReturnDto equipmentReturn);
    }
}
