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
        Task<IEnumerable<Equipment>> GetEquipmentAsync(Expression<Func<Equipment, bool>> where);

        Task<bool> DeleteEquipmentAsync(Equipment equipment);

        Task<bool> UpdateEquipmentAsync(Equipment equipment);

        Task<bool> AddEquipmentAsync(Equipment equipment);
    }
}
