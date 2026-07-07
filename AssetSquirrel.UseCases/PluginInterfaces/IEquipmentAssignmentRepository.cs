using AssetSquirrel.CoreBusiness;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IEquipmentAssignmentRepository
    {
        Task<List<int>> GetAssignedEquipmentIdsAsync();
        Task<IEnumerable<EquipmentAssignment>> GetOpenAssignmentsAsync(Expression<Func<EquipmentAssignment, bool>> where);
    }
}
