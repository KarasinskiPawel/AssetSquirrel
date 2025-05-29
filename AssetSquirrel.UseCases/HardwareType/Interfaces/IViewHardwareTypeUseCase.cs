using AssetSquirrel.CoreBusiness.Dto;
using System.Linq.Expressions;

namespace AssetSquirrel.UseCases.HardwareType.Interfaces
{
    public interface IViewHardwareTypeUseCase
    {
        Task<bool> DeleteHardwareTypeAsync(HardwareTypeDto hardwareType);
        Task<List<HardwareTypeDto>> GetHardwareTypesAsync(Expression<Func<CoreBusiness.HardwareType, bool>> where);
        Task<bool> UpdateHardwareType(HardwareTypeDto hardwareType);
    }
}