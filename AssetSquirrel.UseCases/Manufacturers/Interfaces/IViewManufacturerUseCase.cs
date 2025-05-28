using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System.Linq.Expressions;

namespace AssetSquirrel.UseCases.Manufacturers.Interfaces
{
    public interface IViewManufacturerUseCase
    {
        Task<bool> Deletemanufacturer(ManufacturerDto manufacturer);
        Task<IEnumerable<ManufacturerDto>> GetManufacturersAsync(Expression<Func<Manufacturer>> where);
        Task<bool> UpdateManufacturer(ManufacturerDto manufacturer);
    }
}