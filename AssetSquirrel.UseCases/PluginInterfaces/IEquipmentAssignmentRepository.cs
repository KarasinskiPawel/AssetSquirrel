using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IEquipmentAssignmentRepository
    {
        Task<List<int>> GetAssignedEquipmentIdsAsync();
    }
}
