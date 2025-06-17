using AssetsSquirrel.CoreBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.CoreBusiness
{
    public class EquipmentHistory
    {
        [Key]
        public int EquipmentHistoryId { get; set; }
        public int EquipmentId { get; set; }
        public int? SuppilerId { get; set; }
        public int? ManufacturerId { get; set; }
        public int? HardwareTypeId { get; set; }
        public int? InvoiceId { get; set; }
        public string? ModelName { get; set; }
        public string? SerialNumber { get; set; }
        public string? Description { get; set; }
        public DateTime? DateAdd { get; set; }
        public DateTime? DateRemoved { get; set; }
        public DateTime DateOfChange { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }

        //Navigation
        public Suppiler? Suppiler { get; set; }
        public Manufacturer? Manufacturer { get; set; }
        public HardwareType? HardwareType { get; set; }
        public Invoice? Invoice { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        // Relation N:1
        public Equipment? Equipment { get; set; }
    }
}
