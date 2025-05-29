using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.HardwareType.Interfaces
{
    public interface IEditHardwareTypeUseCase
    {
        Task<bool> UpdateHardwareTypeAsync(HardwareTypeDto hardwareType);
    }
}