using AssetSquirrel.CoreBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Locations.Interfaces
{
    public interface IViewLocations
    {
        Task<IEnumerable<Location>> ExecuteAsync(Expression<Func<Location, bool>> where);
    }
}
