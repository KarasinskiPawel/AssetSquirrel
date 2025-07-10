using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.CoreBusiness
{
    public class EquipmentHandoverDetail
    {
        [Key]
        public int EquipmentHandoverDetailId { get; set; }
        public int EquipmentHandoverId { get; set; }
        public EquipmentHandover? EquipmentHandover { get; set; }
        public int HardwareTypeId { get; set; }
        public HardwareType? HardwareType { get; set; }
        public string? Comment { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
