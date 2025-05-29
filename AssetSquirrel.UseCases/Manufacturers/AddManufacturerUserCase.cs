using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Manufacturers.Interfaces;
using AssetSquirrel.UseCases.Mapper;
using AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Manufacturers
{
    public class AddManufacturerUserCase : IAddManufacturerUserCase
    {
        private readonly IManufacturersRepository manufacturersRepository;
        public AddManufacturerUserCase(IManufacturersRepository manufacturersRepository)
        {
            this.manufacturersRepository = manufacturersRepository;
        }
        public async Task<bool> AddManufacturerAsync(ManufacturerDto manufacturer)
        {
            return await manufacturersRepository.AddManufacturerAsync(
                new GenericMapper<Manufacturer, ManufacturerDto>().Map(manufacturer)
                );
        }
    }
}
