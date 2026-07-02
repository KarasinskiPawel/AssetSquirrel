using AssetSquirrel.CoreBusiness;
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
    public class EditHardwareTypeUseCase : IEditHardwareTypeUseCase
    {
        private readonly IHardwareTypeRepository hardwareTypeRepository;

        public EditHardwareTypeUseCase(IHardwareTypeRepository hardwareTypeRepository)
        {
            this.hardwareTypeRepository = hardwareTypeRepository;
        }

        public async Task<Result<CoreBusiness.Dto.HardwareTypeDto>> UpdateHardwareTypeAsync(CoreBusiness.Dto.HardwareTypeDto hardwareType)
        {
            var result = await hardwareTypeRepository.UpdateHardwareTypeAsync(hardwareType.Adapt<CoreBusiness.HardwareType>());

            return result.Select(ht => ht.Adapt<CoreBusiness.Dto.HardwareTypeDto>());
        }
    }
}
