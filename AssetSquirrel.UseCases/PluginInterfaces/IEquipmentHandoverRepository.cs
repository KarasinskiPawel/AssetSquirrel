using AssetSquirrel.CoreBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IEquipmentHandoverRepository
    {
        Task<Result<CoreBusiness.EquipmentHandover>> AddEquipmentHandoverAsync(CoreBusiness.EquipmentHandover equipmentHandover);
        Task<Result<CoreBusiness.EquipmentHandover>> DeleteEquipmentHandoverAsync(CoreBusiness.EquipmentHandover equipmentHandover);
        Task<IEnumerable<CoreBusiness.EquipmentHandover>> GetEquipmentHandoversAsync(Expression<Func<CoreBusiness.EquipmentHandover, bool>> where);
        Task<Result<CoreBusiness.EquipmentHandover>> UpdateEquipmentHandoverAsync(CoreBusiness.EquipmentHandover equipmentHandover);
        Task<Result<CoreBusiness.EquipmentHandover>> PostEquipmentHandoverAsync(CoreBusiness.EquipmentHandover equipmentHandover, string preparedByUserId);
        Task<Result<CoreBusiness.EquipmentHandover>> CancelEquipmentHandoverAsync(int equipmentHandoverId, string cancelledByUserId);
    }
}
