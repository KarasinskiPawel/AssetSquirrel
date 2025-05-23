using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.Locations.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using AssetSquirrel.UseCases.Mapper;

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
            return new GenericMapper<LocationDto, Location>().Map(
                await locationRepository.GetLocationsAsync(where)
            ).ToList();
        }

        public async Task<bool> UpdateLocationAsync(LocationDto location)
        {
            return await locationRepository.UpdateLocationAsync(
                new GenericMapper<Location, LocationDto>().Map(location)
                );
        }

        public async Task<bool> DeleteLocationAync(LocationDto location)
        {
            return await locationRepository.DeleteLocationAsync(
                new GenericMapper<Location, LocationDto>().Map(location)
                );
        }
    }
}
