using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IEquipmentRepository
    {
        Task<List<EquipmentDto>> GetEquipmentAsync(Expression<Func<Equipment, bool>> where);

        // Highest InventoryNumber currently in the table, or null if there is none yet.
        Task<string?> GetLastInventoryNumberAsync();

        Task<Result<Equipment>> DeleteEquipmentAsync(Equipment equipment);

        Task<Result<Equipment>> UpdateEquipmentAsync(Equipment equipment);

        Task<Result<Equipment>> AddEquipmentAsync(Equipment equipment);
    }
}
