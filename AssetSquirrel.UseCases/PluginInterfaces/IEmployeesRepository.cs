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
        Task<bool> AddEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(Employee employee);
        Task<IEnumerable<Employee>> GetEmployeesAsync(Expression<Func<Employee, bool>> where);
        Task<bool> UpdateEmployeeAsync(Employee employee);
    }
}
