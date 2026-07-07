using AssetSquirrel.CoreBusiness;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IEquipmentReturnRepository
    {
        Task<IEnumerable<CoreBusiness.EquipmentReturn>> GetEquipmentReturnsAsync(Expression<Func<CoreBusiness.EquipmentReturn, bool>> where);
        Task<Result<CoreBusiness.EquipmentReturn>> PostEquipmentReturnAsync(CoreBusiness.EquipmentReturn equipmentReturn, List<int> equipmentAssignmentIds, string preparedByUserId);
        Task<Result<CoreBusiness.EquipmentReturn>> UpdateEquipmentReturnAsync(CoreBusiness.EquipmentReturn equipmentReturn);
    }
}
