using AssetSquirrel.CoreBusiness;

namespace AssetSquirrel.UseCases.Locations.Interfaces
{
    public interface IAddLocationsUseCase
    {
        Task AddLocationAsync(Location location);
    }
}