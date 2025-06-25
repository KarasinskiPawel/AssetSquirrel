using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Mapper;
using AssetSquirrel.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.EquipmentUseCase
{
    public class EditEquipmentUseCase
    {
        private readonly IEquipmentRepository equipmentRepository;

        public EditEquipmentUseCase(IEquipmentRepository equipmentRepository)
        {
            this.equipmentRepository = equipmentRepository;
        }

        public async Task<bool> UpdateEquipmentAsync(EquipmentDto equipment)
        {
            return await equipmentRepository.UpdateEquipmentAsync(
                new GenericMapper<Equipment, EquipmentDto>().Map(equipment)
                );
        }
    }
}
