using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.HardwareType.Interfaces;
using AssetSquirrel.UseCases.Mapper;
using AssetSquirrel.UseCases.PluginInterfaces;
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

        public async Task<bool> AddHardwareTypeAsync(HardwareTypeDto hardwareType)
        {
            return await hardwareTypeRepository.AddHardwareTypeAsync(
                new GenericMapper<CoreBusiness.HardwareType, HardwareTypeDto>().Map(hardwareType)
                );
        }
    }
}
