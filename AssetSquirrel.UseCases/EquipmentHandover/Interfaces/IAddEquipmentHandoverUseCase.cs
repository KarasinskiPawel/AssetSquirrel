using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System.Linq.Expressions;

namespace AssetSquirrel.UseCases.EquipmentHandover.Interfaces
{
    public interface IAddEquipmentHandoverUseCase
    {
        Task<List<EmployeeDto>> GetEmployeesAsync(Expression<Func<Employee, bool>> where);
        Task<List<EquipmentDto>> GetEquipmentAsync(Expression<Func<Equipment, bool>> where);
        Task<List<LocationDto>> GetLocationsAsync(Expression<Func<Location, bool>> where);
    }
}