using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.HardwareType.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.HardwareType
{
    public class AddHardwareTypeUseCase : IAddHardwareTypeUseCase
    {
        private readonly IHardwareTypeRepository hardwareTypeRepository;

        public AddHardwareTypeUseCase(IHardwareTypeRepository hardwareTypeRepository)
        {
            this.hardwareTypeRepository = hardwareTypeRepository;
        }

        public async Task<Result<HardwareTypeDto>> AddHardwareTypeAsync(HardwareTypeDto hardwareType)
        {
            var result = await hardwareTypeRepository.AddHardwareTypeAsync(hardwareType.Adapt<CoreBusiness.HardwareType>());

            return result.Select(ht => ht.Adapt<HardwareTypeDto>());
        }
    }
}
