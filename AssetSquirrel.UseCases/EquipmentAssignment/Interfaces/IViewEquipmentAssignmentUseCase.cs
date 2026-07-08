using AssetSquirrel.CoreBusiness.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.EquipmentAssignment.Interfaces
{
    public interface IViewEquipmentAssignmentUseCase
    {
        Task<List<EquipmentAssignmentOverviewDto>> GetEquipmentAssignmentOverviewAsync(EquipmentAssignmentFilter filter);
    }
}
