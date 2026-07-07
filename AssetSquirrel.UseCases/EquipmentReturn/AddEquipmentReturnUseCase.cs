using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentReturn.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.EquipmentReturn
{
    public class AddEquipmentReturnUseCase : IAddEquipmentReturnUseCase
    {
        private readonly IEmployeesRepository employeesRepository;
        private readonly ILocationRepository locationRepository;
        private readonly IEquipmentAssignmentRepository equipmentAssignmentRepository;
        private readonly IEquipmentReturnRepository equipmentReturnRepository;

        public AddEquipmentReturnUseCase(
            IEmployeesRepository employeesRepository,
            ILocationRepository locationRepository,
            IEquipmentAssignmentRepository equipmentAssignmentRepository,
            IEquipmentReturnRepository equipmentReturnRepository
            )
        {
            this.employeesRepository = employeesRepository;
            this.locationRepository = locationRepository;
            this.equipmentAssignmentRepository = equipmentAssignmentRepository;
            this.equipmentReturnRepository = equipmentReturnRepository;
        }

        public async Task<List<EmployeeDto>> GetEmployeesAsync(Expression<Func<Employee, bool>> where)
        {
            return (await employeesRepository.GetEmployeesAsync(where)).Adapt<List<EmployeeDto>>();
        }

        public async Task<List<LocationDto>> GetLocationsAsync(Expression<Func<Location, bool>> where)
        {
            return (await locationRepository.GetLocationsAsync(where)).Adapt<List<LocationDto>>();
        }

        public async Task<List<EquipmentAssignmentDto>> GetOpenAssignmentsAsync(int? employeeId, int? locationId)
        {
            if (employeeId is null && locationId is null)
            {
                return new List<EquipmentAssignmentDto>();
            }

            var assignments = await equipmentAssignmentRepository.GetOpenAssignmentsAsync(a =>
                a.DateOfReturn == null
                && ((employeeId != null && a.EmployeeId == employeeId) || (locationId != null && a.LocationId == locationId)));

            return assignments.Select(a => new EquipmentAssignmentDto
            {
                EquipmentAssignmentId = a.EquipmentAssignmentId,
                EquipmentId = a.EquipmentId,
                ManufacturerName = a.Equipment != null && a.Equipment.Manufacturer != null ? a.Equipment.Manufacturer.Name : null,
                HardwareTypeName = a.Equipment != null && a.Equipment.HardwareType != null ? a.Equipment.HardwareType.Name : null,
                ModelName = a.Equipment != null ? a.Equipment.ModelName : null,
                SerialNumber = a.Equipment != null ? a.Equipment.SerialNumber : null,
                LocationId = a.LocationId,
                Location = a.Location,
                EmployeeId = a.EmployeeId,
                Employee = a.Employee,
                DateOfHandover = a.DateOfHandover,
                DateOfReturn = a.DateOfReturn
            }).ToList();
        }

        public async Task<Result<EquipmentReturnDto>> SaveReturnAsync(EquipmentReturnDto equipmentReturn, List<int> equipmentAssignmentIds, string preparedByUserId)
        {
            var entity = equipmentReturn.Adapt<AssetSquirrel.CoreBusiness.EquipmentReturn>();

            var result = await equipmentReturnRepository.PostEquipmentReturnAsync(entity, equipmentAssignmentIds, preparedByUserId);

            return result.Select(r => r.Adapt<EquipmentReturnDto>());
        }
    }
}
