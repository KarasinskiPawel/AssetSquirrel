using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.Locations.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Locations
{
    public class ViewLocations : IViewLocations
    {
        private readonly ILocationRepository locationRepository;

        public ViewLocations(ILocationRepository locationRepository)
        {
            this.locationRepository = locationRepository;
        }
        public async Task<IEnumerable<Location>> ExecuteAsync(Expression<Func<Location, bool>> where)
        {
            return await locationRepository.GetLocationsAsync(where);
        }
    }
}
