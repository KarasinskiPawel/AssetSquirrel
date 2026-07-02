using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Employees.Interfaces
{
    public interface IViewEmployeesUseCase
    {
        Task<Result<EmployeeDto>> DeleteEmployeeAsync(EmployeeDto employee);
        Task<List<EmployeeDto>> GetEmployeesAsync(Expression<Func<CoreBusiness.Employee, bool>> where);
        Task<Result<EmployeeDto>> UpdateEmployee(EmployeeDto employee);
    }
}
