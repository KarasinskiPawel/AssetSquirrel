using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.Locations.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Mapster;
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

        public async Task<Result<LocationDto>> UpdateLocationAsync(LocationDto location)
        {
            var result = await locationRepository.UpdateLocationAsync(location.Adapt<Location>());

            return result.Select(l => l.Adapt<LocationDto>());
        }
    }
}
