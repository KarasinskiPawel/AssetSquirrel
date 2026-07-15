using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IEquipmentRepository
    {
        Task<List<EquipmentDto>> GetEquipmentAsync(Expression<Func<Equipment, bool>> where);

        // Count and page are separate so a Virtualize ItemsProvider can fetch the
        // count once per filter change and reuse it across every scroll-triggered
        // page fetch, instead of recomputing it (an expensive query on its own) on
        // every single batch.
        Task<int> GetEquipmentCountAsync(Expression<Func<Equipment, bool>> where, CancellationToken cancellationToken = default);
        Task<List<EquipmentDto>> GetEquipmentPageAsync(Expression<Func<Equipment, bool>> where, int startIndex, int count, string? sortColumn = null, bool sortDescending = false, CancellationToken cancellationToken = default);

        // Highest InventoryNumber currently in the table, or null if there is none yet.
        Task<string?> GetLastInventoryNumberAsync();

        Task<Result<Equipment>> DeleteEquipmentAsync(Equipment equipment);

        Task<Result<Equipment>> UpdateEquipmentAsync(Equipment equipment);

        Task<Result<Equipment>> AddEquipmentAsync(Equipment equipment);
    }
}
