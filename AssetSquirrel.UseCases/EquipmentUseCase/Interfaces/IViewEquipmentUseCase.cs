using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System.Linq.Expressions;

namespace AssetSquirrel.UseCases.EquipmentUseCase.Interfaces
{
    public interface IViewEquipmentUseCase
    {
        Task<bool> DeleteEquipmentAsync(EquipmentDto equipment);
        Task<List<EquipmentDto>> GetEquipmentAsync(Expression<Func<Equipment, bool>> where);
        Task<bool> UpdateEquipmentAsync(EquipmentDto equipment);
    }
}