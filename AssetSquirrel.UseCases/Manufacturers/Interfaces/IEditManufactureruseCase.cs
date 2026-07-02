using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Manufacturers.Interfaces
{
    public interface IEditManufactureruseCase
    {
        Task<Result<ManufacturerDto>> UpdateManufacturerAsync(ManufacturerDto manufacturer);
    }
}