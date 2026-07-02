using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Employees.Interfaces
{
    public interface IAddEmployeeUseCase
    {
        Task<Result<EmployeeDto>> AddEmployeeAsync(EmployeeDto employee);
    }
}