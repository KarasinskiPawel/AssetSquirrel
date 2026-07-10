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
            var handovers = await equipmentHandoverRepository.GetEquipmentHandoversAsync(where);

            return handovers.Select(h => new EquipmentHandoverDto
            {
                EquipmentHandoverId = h.EquipmentHandoverId,
                HandoverDocumentNumber = h.HandoverDocumentNumber,
                FromLocationId = h.FromLocationId,
                ToLocationId = h.ToLocationId,
                ToLocation = h.ToLocation,
                FromEmployeeId = h.FromEmployeeId,
                ToEmployeeId = h.ToEmployeeId,
                ToEmployee = h.ToEmployee,
                HandoverDate = h.HandoverDate,
                Comment = h.Comment,
                IsPosted = h.IsPosted,
                IsActive = h.IsActive,
                PreparedByUserId = h.PreparedByUserId,
                PreparedByUserName = h.PreparedByUser != null ? h.PreparedByUser.UserName : null,
                FilePath = h.FilePath,
                UploadDate = h.UploadDate,
                EquipmentHandoverDetails = h.EquipmentHandoverDetails.Select(d => new EquipmentHandoverDetailDto
                {
                    EquipmentHandoverDetailId = d.EquipmentHandoverDetailId,
                    EquipmentHandoverId = d.EquipmentHandoverId,
                    EquipmentId = d.EquipmentId,
                    ModelName = d.Equipment != null ? d.Equipment.ModelName : null,
                    SerialNumber = d.Equipment != null ? d.Equipment.SerialNumber : null,
                    InventoryNumber = d.Equipment != null ? d.Equipment.InventoryNumber : null,
                    ManufacturerName = d.Equipment != null && d.Equipment.Manufacturer != null ? d.Equipment.Manufacturer.Name : null,
                    HardwareTypeName = d.Equipment != null && d.Equipment.HardwareType != null ? d.Equipment.HardwareType.Name : null,
                    Comment = d.Comment,
                    IsActive = d.IsActive
                }).ToList()
            }).ToList();
        }
    }
}
