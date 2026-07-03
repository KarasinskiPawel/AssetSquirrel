using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.CoreBusiness.Dto
{
    public class EquipmentHandoverDetailDto
    {
        public int EquipmentHandoverDetailId { get; set; }
        public int EquipmentHandoverId { get; set; }
        public int EquipmentId { get; set; }
        public string? ModelName { get; set; }
        public string? SerialNumber { get; set; }
        public string? ManufacturerName { get; set; }
        public string? HardwareTypeName { get; set; }
        public string? Comment { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
