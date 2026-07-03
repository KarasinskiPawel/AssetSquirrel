using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentUseCase.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.EquipmentUseCase
{
    public class EditEquipmentUseCase : IEditEquipmentUseCase
    {
        private readonly IEquipmentRepository equipmentRepository;
        private readonly IHardwareTypeRepository hardwareTypeRepository;
        private readonly IManufacturersRepository manufacturersRepository;
        private readonly ISuppilersRepository suppilersRepository;
        private readonly IInvoiceRepository invoiceRepository;
        private readonly ILocationRepository locationRepository;

        public EditEquipmentUseCase(
            IEquipmentRepository equipmentRepository,
            IHardwareTypeRepository hardwareTypeRepository,
            IManufacturersRepository manufacturersRepository,
            ISuppilersRepository suppilersRepository,
            IInvoiceRepository invoiceRepository,
            ILocationRepository locationRepository)
        {
            this.equipmentRepository = equipmentRepository;
            this.hardwareTypeRepository = hardwareTypeRepository;
            this.manufacturersRepository = manufacturersRepository;
            this.suppilersRepository = suppilersRepository;
            this.invoiceRepository = invoiceRepository;
            this.locationRepository = locationRepository;
        }

        public async Task<Result<EquipmentDto>> UpdateEquipmentAsync(EquipmentDto equipment)
        {
            var result = await equipmentRepository.UpdateEquipmentAsync(equipment.Adapt<Equipment>());

            return result.Select(e => e.Adapt<EquipmentDto>());
        }

        public async Task<List<LocationDto>> GetLocationsAsync(Expression<Func<Location, bool>> where)
        {
            return (await locationRepository.GetLocationsAsync(where)).Adapt<List<LocationDto>>();
        }

        public Task<List<HardwareTypeDto>> GetHardwareTypesAsync(Expression<Func<CoreBusiness.HardwareType, bool>> where)
        {
            return hardwareTypeRepository.GetHardwareTypesAsync(where)
                .ContinueWith(task => task.Result.Adapt<List<HardwareTypeDto>>());
        }

        public Task<List<ManufacturerDto>> GetManufacturersAsync(Expression<Func<Manufacturer, bool>> where)
        {
            return manufacturersRepository.GetManufacturersAsync(where)
                .ContinueWith(task => task.Result.Adapt<List<ManufacturerDto>>());
        }

        public Task<List<SuppilerDto>> GetSuppilersAsync(Expression<Func<Suppiler, bool>> where)
        {
            return suppilersRepository.GetSuppilersAsync(where)
                .ContinueWith(task => task.Result.Adapt<List<SuppilerDto>>());
        }

        public async Task<List<InvoiceDto>> GetInvoicesAsync(Expression<Func<Invoice, bool>> where)
        {
            return await invoiceRepository.GetInvoicesAsync(where);
        }
    }
}
