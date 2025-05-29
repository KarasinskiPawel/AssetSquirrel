using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Manufacturers.Interfaces
{
    public interface IAddManufacturerUserCase
    {
        Task<bool> AddManufacturerAsync(ManufacturerDto manufacturer);
    }
}