using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System.Linq.Expressions;
using System.Threading;

namespace AssetSquirrel.UseCases.EquipmentUseCase.Interfaces
{
    public interface IViewEquipmentUseCase
    {
        Task<Result<EquipmentDto>> DeleteEquipmentAsync(EquipmentDto equipment);
        Task<List<EquipmentDto>> GetEquipmentAsync(Expression<Func<Equipment, bool>> where);
        Task<int> GetEquipmentCountAsync(Expression<Func<Equipment, bool>> where, CancellationToken cancellationToken = default);
        Task<List<EquipmentDto>> GetEquipmentPageAsync(Expression<Func<Equipment, bool>> where, int startIndex, int count, string? sortColumn = null, bool sortDescending = false, CancellationToken cancellationToken = default);
        Task<Result<EquipmentDto>> UpdateEquipmentAsync(EquipmentDto equipment);
    }
}