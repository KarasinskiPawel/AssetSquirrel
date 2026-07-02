using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Employees.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Employees
{
    public class ViewEmployeesUseCase : IViewEmployeesUseCase
    {
        private readonly IEmployeesRepository employeesRepository;

        public ViewEmployeesUseCase(IEmployeesRepository employeesRepository)
        {
            this.employeesRepository = employeesRepository;
        }
        public async Task<Result<EmployeeDto>> DeleteEmployeeAsync(EmployeeDto employee)
        {
            var result = await employeesRepository.DeleteEmployeeAsync(employee.Adapt<Employee>());

            return result.Select(e => e.Adapt<EmployeeDto>());
        }

        public async Task<List<EmployeeDto>> GetEmployeesAsync(Expression<Func<Employee, bool>> where)
        {
            return (await employeesRepository.GetEmployeesAsync(where)).Adapt<List<EmployeeDto>>();
        }

        public async Task<Result<EmployeeDto>> UpdateEmployee(EmployeeDto employee)
        {
            var result = await employeesRepository.UpdateEmployeeAsync(employee.Adapt<Employee>());

            return result.Select(e => e.Adapt<EmployeeDto>());
        }
    }
}
