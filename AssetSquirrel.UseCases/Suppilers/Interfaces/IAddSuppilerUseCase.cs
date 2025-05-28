using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Suppilers.Interfaces
{
    public interface IAddSuppilerUseCase
    {
        Task<bool> AddSuppilerAsync(SuppilerDto suppiler);
    }
}