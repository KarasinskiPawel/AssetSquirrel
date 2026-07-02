using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Suppilers.Interfaces
{
    public interface IEditSuppilerUseCase
    {
        Task<Result<SuppilerDto>> UpdateSuppilerAsync(SuppilerDto suppiler);
    }
}