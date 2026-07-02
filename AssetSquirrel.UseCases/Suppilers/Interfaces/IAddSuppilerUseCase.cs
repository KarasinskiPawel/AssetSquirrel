using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Suppilers.Interfaces
{
    public interface IAddSuppilerUseCase
    {
        Task<Result<SuppilerDto>> AddSuppilerAsync(SuppilerDto suppiler);
    }
}