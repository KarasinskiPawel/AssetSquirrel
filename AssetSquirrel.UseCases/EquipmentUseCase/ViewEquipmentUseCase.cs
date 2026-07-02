using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentUseCase.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Mapster;

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

        public async Task<Result<EquipmentDto>> DeleteEquipmentAsync(EquipmentDto equipment)
        {
            var result = await equipmentRepository.DeleteEquipmentAsync(equipment.Adapt<Equipment>());

            return result.Select(e => e.Adapt<EquipmentDto>());
        }

        public async Task<Result<EquipmentDto>> UpdateEquipmentAsync(EquipmentDto equipment)
        {
            var result = await equipmentRepository.UpdateEquipmentAsync(equipment.Adapt<Equipment>());

            return result.Select(e => e.Adapt<EquipmentDto>());
        }
    }
}
