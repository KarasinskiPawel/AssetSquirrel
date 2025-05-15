using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.CoreBusiness
{
    public class Equipment
    {
        [Key]
        public int EquipmentId { get; set; }
        public int? SuppilerId { get; set; }
        public int? ManufacturerId { get; set; }
        public int? HardwareTypeId { get; set; }
        public string? ModelName { get; set; }
        public bool IsActive { get; set; }

        //Navigation
        public Suppiler? Suppiler { get; set; }
        public Manufacturer? Manufacturer { get; set; }
        public HardwareType? HardwareType { get; set; }
    }
}
