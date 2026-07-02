using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.HardwareType.Interfaces
{
    public interface IEditHardwareTypeUseCase
    {
        Task<Result<HardwareTypeDto>> UpdateHardwareTypeAsync(HardwareTypeDto hardwareType);
    }
}