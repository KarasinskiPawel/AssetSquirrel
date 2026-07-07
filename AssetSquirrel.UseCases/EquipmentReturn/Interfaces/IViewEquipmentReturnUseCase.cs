using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.EquipmentReturn.Interfaces
{
    public interface IViewEquipmentReturnUseCase
    {
        Task<List<EquipmentReturnDto>> GetEquipmentReturnsAsync(Expression<Func<AssetSquirrel.CoreBusiness.EquipmentReturn, bool>> where);
    }
}
