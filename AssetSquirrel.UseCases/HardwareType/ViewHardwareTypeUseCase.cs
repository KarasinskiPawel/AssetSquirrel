using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.HardwareType.Interfaces;
using AssetSquirrel.UseCases.Mapper;
using AssetSquirrel.UseCases.PluginInterfaces;
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
            return [.. new GenericMapper<HardwareTypeDto, AssetSquirrel.CoreBusiness.HardwareType>().Map(
                await hardwareTypeRepository.GetHardwareTypesAsync(where)
                )];
        }

        public async Task<bool> UpdateHardwareType(HardwareTypeDto hardwareType)
        {
            return await hardwareTypeRepository.UpdateHardwareTypeAsync(
                new GenericMapper<AssetSquirrel.CoreBusiness.HardwareType, HardwareTypeDto>().Map(hardwareType)
                );
        }

        public async Task<bool> DeleteHardwareTypeAsync(HardwareTypeDto hardwareType)
        {
            return await hardwareTypeRepository.DeleteHardwareTypeAsync(
                new GenericMapper<AssetSquirrel.CoreBusiness.HardwareType, HardwareTypeDto>().Map(hardwareType)
                );
        }
    }
}
