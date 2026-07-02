using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Employees.Interfaces
{
    public interface IEditEmployeeUseCase
    {
        Task<Result<EmployeeDto>> EditEmployeeAsync(EmployeeDto employee);
    }
}