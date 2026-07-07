using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.EquipmentReturn.Interfaces
{
    public interface IEquipmentReturnPdfGenerator
    {
        byte[] Generate(EquipmentReturnDto equipmentReturn);
    }
}
