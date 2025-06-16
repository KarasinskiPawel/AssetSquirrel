using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Employees.Interfaces
{
    public interface IAddEmployeeUseCase
    {
        Task<bool> AddEmployeeAsync(EmployeeDto employee);
    }
}