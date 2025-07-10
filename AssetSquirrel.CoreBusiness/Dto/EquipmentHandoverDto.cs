using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.CoreBusiness.Dto
{
    public class EquipmentHandoverDto
    {
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

        public IEnumerable<EquipmentHandoverDetail> EquipmentHandoverDetails { get; set; } = new List<EquipmentHandoverDetail>();
    }
}
