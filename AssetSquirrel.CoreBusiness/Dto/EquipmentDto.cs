using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.CoreBusiness.Dto
{
    public class EquipmentDto
    {
        public int EquipmentId { get; set; }
        [Required]
        public int SuppilerId { get; set; }
        public string? SuppilerName { get; set; }
        [Required]
        public int ManufacturerId { get; set; }
        public string? ManufacturerName { get; set; }
        [Required]
        public int HardwareTypeId { get; set; }
        public string? HardwareTypeName { get; set; }   
        public int? InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        [Required]
        [MinLength(5, ErrorMessage = "Min length 5 chars.")]
        public string ModelName { get; set; } = string.Empty;
        [Required]
        public string? SerialNumber { get; set; }
        public string? Description { get; set; }
        public DateTime DateAdd { get; set; } = DateTime.Now;
        public DateTime? DateRemoved { get; set; }
        public bool IsActive { get; set; } = true;

        public string? UserId { get; set; } // Foreign key to ApplicationUser
        public string? UserName { get; set; }
    }
}
