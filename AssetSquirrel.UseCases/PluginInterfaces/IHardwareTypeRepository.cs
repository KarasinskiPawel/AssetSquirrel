using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IHardwareTypeRepository
    {
        Task<IEnumerable<CoreBusiness.HardwareType>> GetHardwareTypesAsync(Expression<Func<CoreBusiness.HardwareType, bool>> where);
        Task<bool> DeleteHardwareTypeAsync(CoreBusiness.HardwareType hardwareType);
        Task<bool> UpdateHardwareTypeAsync(CoreBusiness.HardwareType hardwareType);
        Task<bool> AddHardwareTypeAsync(CoreBusiness.HardwareType hardwareType);
    }
}
