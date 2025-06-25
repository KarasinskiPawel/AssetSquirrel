using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentUseCase.Interfaces;
using AssetSquirrel.UseCases.Mapper;
using AssetSquirrel.UseCases.PluginInterfaces;

namespace AssetSquirrel.UseCases.EquipmentUseCase
{
    public class ViewEquipmentUseCase : IViewEquipmentUseCase
    {
        private readonly IEquipmentRepository equipmentRepository;

        public ViewEquipmentUseCase(IEquipmentRepository equipmentRepository)
        {
            this.equipmentRepository = equipmentRepository;
        }

        public async Task<List<EquipmentDto>> GetEquipmentAsync(Expression<Func<Equipment, bool>> where)
        {
            return await equipmentRepository.GetEquipmentAsync(where);
        }

        public async Task<bool> DeleteEquipmentAsync(EquipmentDto equipment)
        {
            return await equipmentRepository.DeleteEquipmentAsync(
                new GenericMapper<Equipment, EquipmentDto>().Map(equipment)
                );
        }

        public async Task<bool> UpdateEquipmentAsync(EquipmentDto equipment)
        {
            return await equipmentRepository.UpdateEquipmentAsync(
                new GenericMapper<Equipment, EquipmentDto>().Map(equipment)
                );
        }
    }
}
