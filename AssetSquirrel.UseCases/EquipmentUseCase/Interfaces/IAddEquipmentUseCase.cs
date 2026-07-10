using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System.Linq.Expressions;

namespace AssetSquirrel.UseCases.EquipmentUseCase.Interfaces
{
    public interface IAddEquipmentUseCase
    {
        Task<Result<EquipmentDto>> AddEquipmentAsync(EquipmentDto equipment);

        // Suggested next inventory number, for the Add form to pre-fill on load.
        Task<string> GetNextInventoryNumberAsync();

        Task<List<SuppilerDto>> GetSuppilersAsync(Expression<Func<Suppiler, bool>> where);
        Task<List<ManufacturerDto>> GetManufacturersAsync(Expression<Func<Manufacturer, bool>> where);
        Task<List<HardwareTypeDto>> GetHardwareTypesAsync(Expression<Func<CoreBusiness.HardwareType, bool>> where);
        Task<List<InvoiceDto>> GetInvoicesAsync(Expression<Func<Invoice, bool>> where);
        Task<List<LocationDto>> GetLocationsAsync(Expression<Func<Location, bool>> where);
    }
}