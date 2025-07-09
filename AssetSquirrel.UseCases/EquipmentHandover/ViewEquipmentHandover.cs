using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
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
    public class ViewEquipmentHandover
    {
        private readonly IEquipmentHandoverRepository equipmentHandoverRepository;
        private readonly IEquipmentRepository equipmentRepository;
        private readonly ILocationRepository locationRepository;
        private readonly IEmployeesRepository employeesRepository;

        public ViewEquipmentHandover(
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

        public async Task<List<Location>> GetLocationsAsync(Expression<Func<Location, bool>> where)
        {
            return (await locationRepository.GetLocationsAsync(where)).ToList();
        }

        public async Task<List<Employee>> GetEmployeesAsync(Expression<Func<Employee, bool>> where)
        {
            return (await employeesRepository.GetEmployeesAsync(where)).ToList();
        }
    }
}
