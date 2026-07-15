using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
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

        public async Task<int> GetEquipmentCountAsync(Expression<Func<Equipment, bool>> where, CancellationToken cancellationToken = default)
        {
            return await equipmentRepository.GetEquipmentCountAsync(where, cancellationToken);
        }

        public async Task<List<EquipmentDto>> GetEquipmentPageAsync(Expression<Func<Equipment, bool>> where, int startIndex, int count, string? sortColumn = null, bool sortDescending = false, CancellationToken cancellationToken = default)
        {
            return await equipmentRepository.GetEquipmentPageAsync(where, startIndex, count, sortColumn, sortDescending, cancellationToken);
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
