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
        Task<bool> AddEquipmentHandoverAsync(CoreBusiness.EquipmentHandover equipmentHandover);
        Task<bool> DeleteEquipmentHandoverAsync(CoreBusiness.EquipmentHandover equipmentHandover);
        Task<IEnumerable<CoreBusiness.EquipmentHandover>> GetEquipmentHandoversAsync(Expression<Func<CoreBusiness.EquipmentHandover, bool>> where);
        Task<bool> UpdateEquipmentHandoverAsync(CoreBusiness.EquipmentHandover equipmentHandover);
    }
}
