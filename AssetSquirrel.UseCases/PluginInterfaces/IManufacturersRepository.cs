using AssetSquirrel.CoreBusiness;
using System.Linq.Expressions;

namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IManufacturersRepository
    {
        Task<Result<Manufacturer>> AddManufacturerAsync(Manufacturer manufacturer);
        Task<Result<Manufacturer>> DeleteManufacturerAsync(Manufacturer manufacturer);
        Task<IEnumerable<Manufacturer>> GetManufacturersAsync(Expression<Func<Manufacturer, bool>> where);
        Task<Result<Manufacturer>> UpdateManufacturerAsync(Manufacturer manufacturer);
    }
}