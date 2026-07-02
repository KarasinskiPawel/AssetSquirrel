using AssetSquirrel.CoreBusiness;

namespace AssetSquirrel.UseCases.Locations.Interfaces
{
    public interface IAddLocationsUseCase
    {
        Task<Result<LocationDto>> AddLocationAsync(LocationDto location);
    }
}