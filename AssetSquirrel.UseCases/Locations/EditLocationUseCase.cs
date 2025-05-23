using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.Locations.Interfaces;
using AssetSquirrel.UseCases.Mapper;
using AssetSquirrel.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Locations
{
    public class EditLocationUseCase : IEditLocationUseCase
    {
        private readonly ILocationRepository locationRepository;

        public EditLocationUseCase(ILocationRepository locationRepository)
        {
            this.locationRepository = locationRepository;
        }

        public async Task<bool> UpdateLocationAsync(LocationDto location)
        {
            return await locationRepository.UpdateLocationAsync(
                new GenericMapper<Location, LocationDto>().Map(location)
                );
        }
    }
}
