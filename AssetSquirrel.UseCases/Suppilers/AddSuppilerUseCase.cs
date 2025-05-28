using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Mapper;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetSquirrel.UseCases.Suppilers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Suppilers
{
    public class AddSuppilerUseCase : IAddSuppilerUseCase
    {
        private readonly ISuppilersRepository suppilersRepository;

        public AddSuppilerUseCase(ISuppilersRepository suppilersRepository)
        {
            this.suppilersRepository = suppilersRepository;
        }

        public async Task<bool> AddSuppilerAsync(SuppilerDto suppiler)
        {
            return await suppilersRepository.AddSuppilerAsync(
                new GenericMapper<Suppiler, SuppilerDto>().Map(suppiler)
                );
        }
    }
}
