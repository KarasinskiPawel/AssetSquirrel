using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.HardwareType.Interfaces
{
    public interface IAddHardwareTypeuseCase
    {
        Task<bool> AddHardwareTypeAsync(HardwareTypeDto hardwareType);
    }
}