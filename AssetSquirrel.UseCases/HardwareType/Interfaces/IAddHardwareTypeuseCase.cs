using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.HardwareType.Interfaces
{
    public interface IAddHardwareTypeUseCase
    {
        Task<Result<HardwareTypeDto>> AddHardwareTypeAsync(HardwareTypeDto hardwareType);
    }
}