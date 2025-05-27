using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System.Linq.Expressions;

namespace AssetSquirrel.UseCases.Suppilers.Interfaces
{
    public interface IViewSuppilersUseCase
    {
        Task<bool> DeleteSuppiler(SuppilerDto suppiler);
        Task<List<SuppilerDto>> GetSuppilersAsync(Expression<Func<Suppiler, bool>> where);
        Task<bool> UpdateSuppiler(SuppilerDto suppiler);
    }
}