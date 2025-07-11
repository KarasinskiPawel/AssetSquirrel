using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System.Linq.Expressions;

namespace AssetSquirrel.UseCases.EquipmentHandover.Interfaces
{
    public interface IViewEquipmentHandoverUseCase
    {
        Task<List<EquipmentHandoverDto>> GetEquipmentHandoverAsync(Expression<Func<CoreBusiness.EquipmentHandover, bool>> where);
    }
}