using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.CoreBusiness.Dto
{
    public class EquipmentHistoryDto
    {
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
    }
}
