using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System.Linq.Expressions;

namespace AssetSquirrel.UseCases.EquipmentReturn.Interfaces
{
    public interface IAddEquipmentReturnUseCase
    {
        Task<List<EmployeeDto>> GetEmployeesAsync(Expression<Func<Employee, bool>> where);
        Task<List<LocationDto>> GetLocationsAsync(Expression<Func<Location, bool>> where);
        Task<List<EquipmentAssignmentDto>> GetOpenAssignmentsAsync(int? employeeId, int? locationId);
        Task<Result<EquipmentReturnDto>> SaveReturnAsync(EquipmentReturnDto equipmentReturn, List<int> equipmentAssignmentIds, string preparedByUserId);
    }
}
