using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.EquipmentHandover.Interfaces
{
    public interface IEditEquipmentHandoverUseCase
    {
        Task<Result<EquipmentHandoverDto>> UpdateEquipmentHandoverAsync(EquipmentHandoverDto handover);
        Task<Result<EquipmentHandoverDto>> CancelEquipmentHandoverAsync(int equipmentHandoverId, string cancelledByUserId);
    }
}
