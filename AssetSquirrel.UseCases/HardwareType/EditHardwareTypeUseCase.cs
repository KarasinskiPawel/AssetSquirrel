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
    public class EditHardwareTypeUseCase : IEditHardwareTypeUseCase
    {
        private readonly IHardwareTypeRepository hardwareTypeRepository;

        public EditHardwareTypeUseCase(IHardwareTypeRepository hardwareTypeRepository)
        {
            this.hardwareTypeRepository = hardwareTypeRepository;
        }

        public async Task<bool> UpdateHardwareTypeAsync(CoreBusiness.Dto.HardwareTypeDto hardwareType)
        {
            return await hardwareTypeRepository.UpdateHardwareTypeAsync(
                new GenericMapper<CoreBusiness.HardwareType, CoreBusiness.Dto.HardwareTypeDto>().Map(hardwareType)
                );
        }
    }
}
