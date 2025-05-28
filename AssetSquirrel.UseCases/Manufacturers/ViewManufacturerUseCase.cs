using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Manufacturers.Interfaces;
using AssetSquirrel.UseCases.Mapper;
using AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories;
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

        public async Task<IEnumerable<ManufacturerDto>> GetManufacturersAsync(Expression<Func<Manufacturer>> where)
        {
            return new GenericMapper<ManufacturerDto, Manufacturer>().Map(
                await manufacturersRepository.GetManufacturersAsync(where)
                );
        }

        public async Task<bool> UpdateManufacturer(ManufacturerDto manufacturer)
        {
            return await manufacturersRepository.UpdateManufacturerAsync(
                new GenericMapper<Manufacturer, ManufacturerDto>().Map(manufacturer)
                );
        }

        public async Task<bool> Deletemanufacturer(ManufacturerDto manufacturer)
        {
            return await manufacturersRepository.DeleteManufacturerAsync(
                new GenericMapper<Manufacturer, ManufacturerDto>().Map(manufacturer)
                );
        }
    }
}
