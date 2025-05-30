using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.HardwareType.Interfaces
{
    public interface IAddHardwareTypeUseCase
    {
        Task<bool> AddHardwareTypeAsync(HardwareTypeDto hardwareType);
    }
}