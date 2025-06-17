using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentUseCase.Interfaces;
using AssetSquirrel.UseCases.Mapper;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.EquipmentUseCase
{
    public class AddEquipmentUseCase : IAddEquipmentUseCase
    {
        private readonly IEquipmentRepository equipmentRepository;
        private readonly IHardwareTypeRepository hardwareTypeRepository;
        private readonly IManufacturersRepository manufacturersRepository;
        private readonly ISuppilersRepository suppilersRepository;

        public AddEquipmentUseCase(
            IEquipmentRepository equipmentRepository,
            IHardwareTypeRepository hardwareTypeRepository,
            IManufacturersRepository manufacturersRepository,
            ISuppilersRepository suppilersRepository)
        {
            this.equipmentRepository = equipmentRepository;
            this.hardwareTypeRepository = hardwareTypeRepository;
            this.manufacturersRepository = manufacturersRepository;
            this.suppilersRepository = suppilersRepository;
        }

        public async Task<bool> AddEquipmentAsync(EquipmentDto equipment)
        {
            return await equipmentRepository.AddEquipmentAsync(
                new GenericMapper<Equipment, EquipmentDto>().Map(equipment)
                );
        }

        public Task<List<HardwareTypeDto>> GetHardwareTypesAsync(Expression<Func<CoreBusiness.HardwareType, bool>> where)
        {
            return hardwareTypeRepository.GetHardwareTypesAsync(where)
                .ContinueWith(task => task.Result.Select(ht => new GenericMapper<CoreBusiness.HardwareType, HardwareTypeDto>().Map(ht)).ToList());
        }

        public Task<List<ManufacturerDto>> GetManufacturersAsync(Expression<Func<Manufacturer, bool>> where)
        {
            return manufacturersRepository.GetManufacturersAsync(where)
                .ContinueWith(task => task.Result.Select(m => new GenericMapper<Manufacturer, ManufacturerDto>().Map(m)).ToList());
        }

        public Task<List<SuppilerDto>> GetSuppilersAsync(Expression<Func<Suppiler, bool>> where)
        {
            return suppilersRepository.GetSuppilersAsync(where)
                .ContinueWith(task => task.Result.Select(s => new GenericMapper<Suppiler, SuppilerDto>().Map(s)).ToList());
        }
    }
}
