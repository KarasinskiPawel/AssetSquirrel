using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetSquirrel.UseCases.Suppilers.Interfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Suppilers
{
    public class ViewSuppilersUseCase : IViewSuppilersUseCase
    {
        private readonly ISuppilersRepository suppilersRepository;

        public ViewSuppilersUseCase(ISuppilersRepository suppilersRepository)
        {
            this.suppilersRepository = suppilersRepository;
        }

        public async Task<List<SuppilerDto>> GetSuppilersAsync(Expression<Func<Suppiler, bool>> where)
        {
            return (await suppilersRepository.GetSuppilersAsync(where)).Adapt<List<SuppilerDto>>();
        }

        public async Task<Result<SuppilerDto>> UpdateSuppiler(SuppilerDto suppiler)
        {
            var result = await suppilersRepository.UpdateSuppilerAsync(suppiler.Adapt<Suppiler>());

            return result.Select(s => s.Adapt<SuppilerDto>());
        }

        public async Task<Result<SuppilerDto>> DeleteSuppiler(SuppilerDto suppiler)
        {
            var result = await suppilersRepository.DeleteSuppilerAsync(suppiler.Adapt<Suppiler>());

            return result.Select(s => s.Adapt<SuppilerDto>());
        }
    }
}
