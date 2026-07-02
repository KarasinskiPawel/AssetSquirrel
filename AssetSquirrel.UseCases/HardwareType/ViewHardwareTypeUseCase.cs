using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.HardwareType.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.HardwareType
{
    public class ViewHardwareTypeUseCase : IViewHardwareTypeUseCase
    {
        private readonly IHardwareTypeRepository hardwareTypeRepository;

        public ViewHardwareTypeUseCase(IHardwareTypeRepository hardwareTypeRepository)
        {
            this.hardwareTypeRepository = hardwareTypeRepository;
        }

        public async Task<List<HardwareTypeDto>> GetHardwareTypesAsync(Expression<Func<AssetSquirrel.CoreBusiness.HardwareType, bool>> where)
        {
            return (await hardwareTypeRepository.GetHardwareTypesAsync(where)).Adapt<List<HardwareTypeDto>>();
        }

        public async Task<Result<HardwareTypeDto>> UpdateHardwareType(HardwareTypeDto hardwareType)
        {
            var result = await hardwareTypeRepository.UpdateHardwareTypeAsync(hardwareType.Adapt<AssetSquirrel.CoreBusiness.HardwareType>());

            return result.Select(ht => ht.Adapt<HardwareTypeDto>());
        }

        public async Task<Result<HardwareTypeDto>> DeleteHardwareTypeAsync(HardwareTypeDto hardwareType)
        {
            var result = await hardwareTypeRepository.DeleteHardwareTypeAsync(hardwareType.Adapt<AssetSquirrel.CoreBusiness.HardwareType>());

            return result.Select(ht => ht.Adapt<HardwareTypeDto>());
        }
    }
}
