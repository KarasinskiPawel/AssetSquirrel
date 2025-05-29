using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Manufacturers.Interfaces
{
    public interface IEditManufactureruseCase
    {
        Task<bool> UpdateManufacturerAsync(ManufacturerDto manufacturer);
    }
}