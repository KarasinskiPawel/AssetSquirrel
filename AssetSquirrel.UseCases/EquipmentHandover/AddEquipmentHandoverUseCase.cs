using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentHandover.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Mapster;
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
        private readonly IEquipmentAssignmentRepository equipmentAssignmentRepository;
        private readonly IEquipmentHandoverRepository equipmentHandoverRepository;

        public AddEquipmentHandoverUseCase(
            IEquipmentRepository equipmentRepository,
            ILocationRepository locationRepository,
            IEmployeesRepository employeesRepository,
            IEquipmentAssignmentRepository equipmentAssignmentRepository,
            IEquipmentHandoverRepository equipmentHandoverRepository
            )
        {
            this.equipmentRepository = equipmentRepository;
            this.locationRepository = locationRepository;
            this.employeesRepository = employeesRepository;
            this.equipmentAssignmentRepository = equipmentAssignmentRepository;
            this.equipmentHandoverRepository = equipmentHandoverRepository;
        }

        public async Task<List<EquipmentDto>> GetEquipmentAsync(Expression<Func<Equipment, bool>> where)
        {
            var equipment = await equipmentRepository.GetEquipmentAsync(where);
            var assignedEquipmentIds = await equipmentAssignmentRepository.GetAssignedEquipmentIdsAsync();

            return equipment.Where(e => !assignedEquipmentIds.Contains(e.EquipmentId)).ToList();
        }

        public async Task<Result<EquipmentHandoverDto>> SaveHandoverAsync(EquipmentHandoverDto handover, List<int> equipmentIds, string preparedByUserId)
        {
            var entity = handover.Adapt<AssetSquirrel.CoreBusiness.EquipmentHandover>();

            entity.EquipmentHandoverDetails = equipmentIds
                .Select(id => new EquipmentHandoverDetail { EquipmentId = id })
                .ToList();

            var result = await equipmentHandoverRepository.PostEquipmentHandoverAsync(entity, preparedByUserId);

            return result.Select(e => e.Adapt<EquipmentHandoverDto>());
        }

        public async Task<List<LocationDto>> GetLocationsAsync(Expression<Func<Location, bool>> where)
        {
            return (await locationRepository.GetLocationsAsync(where)).Adapt<List<LocationDto>>();
        }

        public async Task<List<EmployeeDto>> GetEmployeesAsync(Expression<Func<Employee, bool>> where)
        {
            return (await employeesRepository.GetEmployeesAsync(where)).Adapt<List<EmployeeDto>>();
        }
    }
}
