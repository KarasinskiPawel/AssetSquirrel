using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System.Linq.Expressions;

namespace AssetSquirrel.UseCases.EquipmentHandover.Interfaces
{
    public interface IViewEquipmentHandover
    {
        Task<List<Employee>> GetEmployeesAsync(Expression<Func<Employee, bool>> where);
        Task<List<EquipmentDto>> GetEquipmentAsync(Expression<Func<Equipment, bool>> where);
        Task<List<EquipmentHandoverDto>> GetEquipmentAsync(Expression<Func<CoreBusiness.EquipmentHandover, bool>> where);
        Task<List<Location>> GetLocationsAsync(Expression<Func<Location, bool>> where);
    }
}