using AssetSquirrel.CoreBusiness;

namespace AssetSquirrel.UseCases.Locations.Interfaces
{
    public interface IEditLocationUseCase
    {
        Task<Result<LocationDto>> UpdateLocationAsync(LocationDto location);
    }
}