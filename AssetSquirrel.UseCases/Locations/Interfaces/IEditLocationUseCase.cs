using AssetSquirrel.CoreBusiness;

namespace AssetSquirrel.UseCases.Locations.Interfaces
{
    public interface IEditLocationUseCase
    {
        Task<bool> UpdateLocationAsync(LocationDto location);
    }
}