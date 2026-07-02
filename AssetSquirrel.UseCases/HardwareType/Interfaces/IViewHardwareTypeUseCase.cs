using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System.Linq.Expressions;

namespace AssetSquirrel.UseCases.HardwareType.Interfaces
{
    public interface IViewHardwareTypeUseCase
    {
        Task<Result<HardwareTypeDto>> DeleteHardwareTypeAsync(HardwareTypeDto hardwareType);
        Task<List<HardwareTypeDto>> GetHardwareTypesAsync(Expression<Func<CoreBusiness.HardwareType, bool>> where);
        Task<Result<HardwareTypeDto>> UpdateHardwareType(HardwareTypeDto hardwareType);
    }
}