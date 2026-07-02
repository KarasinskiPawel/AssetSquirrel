using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentHandover.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.EquipmentHandover
{
    public class ViewEquipmentHandoverUseCase : IViewEquipmentHandoverUseCase
    {
        private readonly IEquipmentHandoverRepository equipmentHandoverRepository;

        public ViewEquipmentHandoverUseCase(
            IEquipmentHandoverRepository equipmentHandoverRepository
            )
        {
            this.equipmentHandoverRepository = equipmentHandoverRepository;
        }

        public async Task<List<EquipmentHandoverDto>> GetEquipmentHandoverAsync(Expression<Func<AssetSquirrel.CoreBusiness.EquipmentHandover, bool>> where)
        {
            return (await equipmentHandoverRepository.GetEquipmentHandoversAsync(where)).Adapt<List<EquipmentHandoverDto>>();
        }
    }
}
