using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetSquirrel.UseCases.Suppilers.Interfaces;
using Mapster;
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

        public async Task<Result<SuppilerDto>> AddSuppilerAsync(SuppilerDto suppiler)
        {
            var result = await suppilersRepository.AddSuppilerAsync(suppiler.Adapt<Suppiler>());

            return result.Select(s => s.Adapt<SuppilerDto>());
        }
    }
}
