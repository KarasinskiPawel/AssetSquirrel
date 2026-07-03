using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.EquipmentHandover.Interfaces
{
    public interface IEquipmentHandoverPdfGenerator
    {
        byte[] Generate(EquipmentHandoverDto handover);
    }
}
