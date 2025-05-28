using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Mapper;
using AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Manufacturers
{
    public class EditManufactureruseCase
    {
        private readonly IManufacturersRepository manufacturersRepository;

        public EditManufactureruseCase(IManufacturersRepository manufacturersRepository)
        {
            this.manufacturersRepository = manufacturersRepository;
        }

        public async Task<bool> UpdateManufacturerAsync(ManufacturerDto manufacturer)
        {
            return await manufacturersRepository.UpdateManufacturerAsync(
                new GenericMapper<Manufacturer, ManufacturerDto>().Map(manufacturer)
                );
        }
    }
}
