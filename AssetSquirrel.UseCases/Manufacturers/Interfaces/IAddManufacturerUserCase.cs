using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Manufacturers.Interfaces
{
    public interface IAddManufacturerUserCase
    {
        Task<Result<ManufacturerDto>> AddManufacturerAsync(ManufacturerDto manufacturer);
    }
}