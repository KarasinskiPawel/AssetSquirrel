using AssetSquirrel.CoreBusiness;
using System.Linq.Expressions;

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories
{
    public interface IManufacturersRepository
    {
        Task<bool> AddManufacturerAsync(Manufacturer manufacturer);
        Task<bool> DeleteManufacturerAsync(Manufacturer manufacturer);
        Task<IEnumerable<Manufacturer>> GetManufacturersAsync(Expression<Func<Manufacturer, bool>> where);
        Task<bool> UpdateManufacturerAsync(Manufacturer manufacturer);
    }
}