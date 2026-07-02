using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Employees.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Employees
{
    public class AddEmployeeUseCase : IAddEmployeeUseCase
    {
        private readonly IEmployeesRepository employeesRepository;

        public AddEmployeeUseCase(IEmployeesRepository employeesRepository)
        {
            this.employeesRepository = employeesRepository;
        }

        public async Task<Result<EmployeeDto>> AddEmployeeAsync(EmployeeDto employee)
        {
            var result = await employeesRepository.AddEmployeeAsync(employee.Adapt<Employee>());

            return result.Select(e => e.Adapt<EmployeeDto>());
        }
    }
}
