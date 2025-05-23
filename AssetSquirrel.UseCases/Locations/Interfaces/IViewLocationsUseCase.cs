using AssetSquirrel.CoreBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Locations.Interfaces
{
    public interface IViewLocationsUseCase
    {
        Task<bool> DeleteLocationAync(LocationDto location);
        Task<List<LocationDto>> GetLocationsAsync(Expression<Func<Location, bool>> where);
        Task<bool> UpdateLocationAsync(LocationDto location);
    }
}
