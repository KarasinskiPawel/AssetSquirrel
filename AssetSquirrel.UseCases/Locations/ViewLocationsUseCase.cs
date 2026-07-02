using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.Locations.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Locations
{
    public class ViewLocationsUseCase : IViewLocationsUseCase
    {
        private readonly ILocationRepository locationRepository;

        public ViewLocationsUseCase(ILocationRepository locationRepository)
        {
            this.locationRepository = locationRepository;
        }
        public async Task<List<LocationDto>> GetLocationsAsync(Expression<Func<Location, bool>> where)
        {
            return (await locationRepository.GetLocationsAsync(where)).Adapt<List<LocationDto>>();
        }

        public async Task<Result<LocationDto>> UpdateLocationAsync(LocationDto location)
        {
            var result = await locationRepository.UpdateLocationAsync(location.Adapt<Location>());

            return result.Select(l => l.Adapt<LocationDto>());
        }

        public async Task<Result<LocationDto>> DeleteLocationAync(LocationDto location)
        {
            var result = await locationRepository.DeleteLocationAsync(location.Adapt<Location>());

            return result.Select(l => l.Adapt<LocationDto>());
        }
    }
}
