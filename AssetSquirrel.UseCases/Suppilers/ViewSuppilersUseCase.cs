using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Mapper;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetSquirrel.UseCases.Suppilers.Interfaces;
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
            return new GenericMapper<SuppilerDto, Suppiler>().Map(
                await suppilersRepository.GetSuppilersAsync(where)
                ).ToList();
        }

        public async Task<bool> UpdateSuppiler(SuppilerDto suppiler)
        {
            return await suppilersRepository.UpdateSuppilerAsync(
                new GenericMapper<Suppiler, SuppilerDto>().Map(suppiler)
                );
        }

        public async Task<bool> DeleteSuppiler(SuppilerDto suppiler)
        {
            return await suppilersRepository.DeleteSuppilerAsync(
                new GenericMapper<Suppiler, SuppilerDto>().Map(suppiler)
                );
        }
    }
}
