using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Employees.Interfaces;
using AssetSquirrel.UseCases.Mapper;
using AssetSquirrel.UseCases.PluginInterfaces;
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
        public async Task<bool> DeleteEmployeeAsync(EmployeeDto employee)
        {
            return await employeesRepository.DeleteEmployeeAsync(
                new GenericMapper<Employee, EmployeeDto>().Map(employee)
                );
        }

        public async Task<List<EmployeeDto>> GetEmployeesAsync(Expression<Func<Employee, bool>> where)
        {
            return new GenericMapper<EmployeeDto, Employee>().Map(
                await employeesRepository.GetEmployeesAsync(where)
                ).ToList();
        }

        public async Task<bool> UpdateEmployee(EmployeeDto employee)
        {
            return await employeesRepository.UpdateEmployeeAsync(
                new GenericMapper<Employee, EmployeeDto>().Map(employee)
                );
        }
    }
}
