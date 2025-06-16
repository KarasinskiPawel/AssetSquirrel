using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Employees.Interfaces
{
    public interface IEditEmployeeUseCase
    {
        Task<bool> EditEmployeeAsync(EmployeeDto employee);
    }
}