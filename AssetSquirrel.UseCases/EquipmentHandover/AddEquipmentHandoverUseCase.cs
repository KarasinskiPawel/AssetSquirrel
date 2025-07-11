using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentHandover.Interfaces;
using AssetSquirrel.UseCases.Mapper;
using AssetSquirrel.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.EquipmentHandover
{
    public class AddEquipmentHandoverUseCase : IAddEquipmentHandoverUseCase
    {
        private readonly IEquipmentRepository equipmentRepository;
        private readonly ILocationRepository locationRepository;
        private readonly IEmployeesRepository employeesRepository;

        public AddEquipmentHandoverUseCase(
            IEquipmentRepository equipmentRepository,
            ILocationRepository locationRepository,
            IEmployeesRepository employeesRepository
            )
        {
            this.equipmentRepository = equipmentRepository;
            this.locationRepository = locationRepository;
            this.employeesRepository = employeesRepository;
        }

        public async Task<List<EquipmentDto>> GetEquipmentAsync(Expression<Func<Equipment, bool>> where)
        {
            return await equipmentRepository.GetEquipmentAsync(where);
        }

        public async Task<List<LocationDto>> GetLocationsAsync(Expression<Func<Location, bool>> where)
        {
            return (new GenericMapper<LocationDto, Location>().Map(
                await locationRepository.GetLocationsAsync(where)).ToList()
                );
        }

        public async Task<List<EmployeeDto>> GetEmployeesAsync(Expression<Func<Employee, bool>> where)
        {
            return (new GenericMapper<EmployeeDto, Employee>().Map(
                await employeesRepository.GetEmployeesAsync(where)).ToList()
                );
        }
    }
}
