using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System.Linq.Expressions;

namespace AssetSquirrel.UseCases.Suppilers.Interfaces
{
    public interface IViewSuppilersUseCase
    {
        Task<Result<SuppilerDto>> DeleteSuppiler(SuppilerDto suppiler);
        Task<List<SuppilerDto>> GetSuppilersAsync(Expression<Func<Suppiler, bool>> where);
        Task<Result<SuppilerDto>> UpdateSuppiler(SuppilerDto suppiler);
    }
}