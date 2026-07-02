using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Manufacturers.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Manufacturers
{
    public class EditManufactureruseCase : IEditManufactureruseCase
    {
        private readonly IManufacturersRepository manufacturersRepository;

        public EditManufactureruseCase(IManufacturersRepository manufacturersRepository)
        {
            this.manufacturersRepository = manufacturersRepository;
        }

        public async Task<Result<ManufacturerDto>> UpdateManufacturerAsync(ManufacturerDto manufacturer)
        {
            var result = await manufacturersRepository.UpdateManufacturerAsync(manufacturer.Adapt<Manufacturer>());

            return result.Select(m => m.Adapt<ManufacturerDto>());
        }
    }
}
