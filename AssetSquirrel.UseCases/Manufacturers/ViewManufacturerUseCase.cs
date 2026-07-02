using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Manufacturers.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Manufacturers
{
    public class ViewManufacturerUseCase : IViewManufacturerUseCase
    {
        private readonly IManufacturersRepository manufacturersRepository;

        public ViewManufacturerUseCase(IManufacturersRepository manufacturersRepository)
        {
            this.manufacturersRepository = manufacturersRepository;
        }

        public async Task<List<ManufacturerDto>> GetManufacturersAsync(Expression<Func<Manufacturer, bool>> where)
        {
            return (await manufacturersRepository.GetManufacturersAsync(where)).Adapt<List<ManufacturerDto>>();
        }

        public async Task<Result<ManufacturerDto>> UpdateManufacturer(ManufacturerDto manufacturer)
        {
            var result = await manufacturersRepository.UpdateManufacturerAsync(manufacturer.Adapt<Manufacturer>());

            return result.Select(m => m.Adapt<ManufacturerDto>());
        }

        public async Task<Result<ManufacturerDto>> Deletemanufacturer(ManufacturerDto manufacturer)
        {
            var result = await manufacturersRepository.DeleteManufacturerAsync(manufacturer.Adapt<Manufacturer>());

            return result.Select(m => m.Adapt<ManufacturerDto>());
        }
    }
}
