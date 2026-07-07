using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.EquipmentReturn.Interfaces
{
    public interface IAddEquipmentReturnDocumentUseCase
    {
        Task<Result<EquipmentReturnDto>> AddEquipmentReturnDocumentAsync(EquipmentReturnDto equipmentReturn, string fileName, string contentType, Stream fileStream);
    }
}
