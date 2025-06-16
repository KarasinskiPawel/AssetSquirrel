using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Employees.Interfaces;
using AssetSquirrel.UseCases.Mapper;
using AssetSquirrel.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Employees
{
    public class EditEmployeeUseCase : IEditEmployeeUseCase
    {
        private readonly IEmployeesRepository employeesRepository;

        public EditEmployeeUseCase(IEmployeesRepository employeesRepository)
        {
            this.employeesRepository = employeesRepository;
        }

        public async Task<bool> EditEmployeeAsync(EmployeeDto employee)
        {
            return await employeesRepository.UpdateEmployeeAsync(
                new GenericMapper<Employee, EmployeeDto>().Map(employee)
                );
        }
    }
}
