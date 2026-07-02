using AssetSquirrel.CoreBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IEmployeesRepository
    {
        Task<Result<Employee>> AddEmployeeAsync(Employee employee);
        Task<Result<Employee>> DeleteEmployeeAsync(Employee employee);
        Task<IEnumerable<Employee>> GetEmployeesAsync(Expression<Func<Employee, bool>> where);
        Task<Result<Employee>> UpdateEmployeeAsync(Employee employee);
    }
}
