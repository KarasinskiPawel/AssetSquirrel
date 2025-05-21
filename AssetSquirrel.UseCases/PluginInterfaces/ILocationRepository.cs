using AssetSquirrel.CoreBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Location>> GetLocationsAsync(Expression<Func<Location, bool>> where);
        Task<bool> UpdateLocationAsync(Location location);
        Task<bool> DeleteLocationAsync(Location location);
        Task<bool> AddLocationAsync(Location location);
    }
}
