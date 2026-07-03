using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.EquipmentHandover.Interfaces
{
    public interface IAddEquipmentHandoverDocumentUseCase
    {
        Task<Result<EquipmentHandoverDto>> AddEquipmentHandoverDocumentAsync(EquipmentHandoverDto handover, string fileName, string contentType, Stream fileStream);
    }
}
