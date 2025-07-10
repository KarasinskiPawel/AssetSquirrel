using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.CoreBusiness
{
    public class EquipmentHandover
    {
        [Key]
        public int EquipmentHandoverId { get; set; }
        public string HandoverDocumentNumber { get; set; } = string.Empty;
        public int FromLocationId { get; set; }
        public Location? FromLocation { get; set; }
        public int? ToLocationId { get; set; }
        public Location? ToLocation { get; set; }
        public int? FromEmployeeId { get; set; }
        public Employee? FromEmployee { get; set; }
        public int ToEmployeeId { get; set; }
        public Employee? ToEmployee { get; set; }
        public DateTime HandoverDate { get; set; }
        public string? Comment { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<EquipmentHandoverDetail> EquipmentHandoverDetails { get; set; } = new List<EquipmentHandoverDetail>();

    }
}
