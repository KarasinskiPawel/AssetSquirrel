using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentReturn.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.EquipmentReturn
{
    public class ViewEquipmentReturnUseCase : IViewEquipmentReturnUseCase
    {
        private readonly IEquipmentReturnRepository equipmentReturnRepository;

        public ViewEquipmentReturnUseCase(IEquipmentReturnRepository equipmentReturnRepository)
        {
            this.equipmentReturnRepository = equipmentReturnRepository;
        }

        public async Task<List<EquipmentReturnDto>> GetEquipmentReturnsAsync(Expression<Func<AssetSquirrel.CoreBusiness.EquipmentReturn, bool>> where)
        {
            var returns = await equipmentReturnRepository.GetEquipmentReturnsAsync(where);

            return returns.Select(r => new EquipmentReturnDto
            {
                EquipmentReturnId = r.EquipmentReturnId,
                ReturnDocumentNumber = r.ReturnDocumentNumber,
                EmployeeId = r.EmployeeId,
                Employee = r.Employee,
                LocationId = r.LocationId,
                Location = r.Location,
                ReturnDate = r.ReturnDate,
                Comment = r.Comment,
                StorageLocationId = r.StorageLocationId,
                StorageLocationName = r.StorageLocation != null ? $"{r.StorageLocation.City} {r.StorageLocation.Street}" : null,
                PreparedByUserId = r.PreparedByUserId,
                PreparedByUserName = r.PreparedByUser != null ? r.PreparedByUser.UserName : null,
                FilePath = r.FilePath,
                UploadDate = r.UploadDate,
                Items = r.EquipmentAssignments.Select(a => new EquipmentAssignmentDto
                {
                    EquipmentAssignmentId = a.EquipmentAssignmentId,
                    EquipmentId = a.EquipmentId,
                    ManufacturerName = a.Equipment != null && a.Equipment.Manufacturer != null ? a.Equipment.Manufacturer.Name : null,
                    HardwareTypeName = a.Equipment != null && a.Equipment.HardwareType != null ? a.Equipment.HardwareType.Name : null,
                    ModelName = a.Equipment != null ? a.Equipment.ModelName : null,
                    SerialNumber = a.Equipment != null ? a.Equipment.SerialNumber : null,
                    InventoryNumber = a.Equipment != null ? a.Equipment.InventoryNumber : null,
                    DateOfHandover = a.DateOfHandover,
                    DateOfReturn = a.DateOfReturn
                }).ToList()
            }).ToList();
        }
    }
}
