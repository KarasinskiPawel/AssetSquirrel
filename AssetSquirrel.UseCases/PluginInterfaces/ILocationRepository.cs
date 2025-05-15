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
        Task UpdateLocationAsync(Location location);
        Task DeleteLocationAsync(Location location);
        Task AddLocationAsync(Location location);
    }
}
