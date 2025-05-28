using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Suppilers.Interfaces
{
    public interface IEditSuppilerUseCase
    {
        Task<bool> UpdateSuppilerAsync(SuppilerDto suppiler);
    }
}