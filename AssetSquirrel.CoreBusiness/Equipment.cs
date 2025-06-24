using AssetsSquirrel.CoreBusiness;
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
        [Required]
        public int SuppilerId { get; set; }
        [Required]
        public int ManufacturerId { get; set; }
        [Required]
        public int HardwareTypeId { get; set; }
        public int? InvoiceId { get; set; }
        [Required]
        [MinLength(5, ErrorMessage = "Min length 5 chars.")]
        public string ModelName { get; set; } = string.Empty;
        [Required]
        public string? SerialNumber { get; set; }
        public string? Description { get; set; }
        public DateTime DateAdd { get; set; } = DateTime.Now;
        public DateTime? DateRemoved { get; set; }
        public bool IsActive { get; set; } = true;

        //Navigation
        public Suppiler? Suppiler { get; set; }
        public Manufacturer? Manufacturer { get; set; }
        public HardwareType? HardwareType { get; set; }
        public Invoice? Invoice { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        // Relation 1:N
        public ICollection<EquipmentHistory>? EquipmentHistories { get; set; }
    }
}
