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
        // Internal asset tag, distinct from the manufacturer's SerialNumber.
        // Auto-generated on add (see AddEquipmentUseCase) as "491" + 8 digits;
        // must stay unique (see EquipmentConfiguration's unique index).
        [Required]
        [MaxLength(11)]
        [RegularExpression(@"^491\d{8}$", ErrorMessage = "Format numeru inwentarzowego: 491 + 8 cyfr.")]
        public string InventoryNumber { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DateAdd { get; set; } = DateTime.Now;
        public DateTime? DateRemoved { get; set; }
        public bool IsAddedToWarehouse { get; set; } = false;
        public bool IsActive { get; set; } = true;

        // Warehouse location the equipment was received into -- restricted
        // to locations flagged as Location.EquipmentStorage in the UI.
        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        //Navigation
        public Suppiler? Suppiler { get; set; }
        public Manufacturer? Manufacturer { get; set; }
        public HardwareType? HardwareType { get; set; }
        public Invoice? Invoice { get; set; }
        // Person who registered/last changed this record in the system — NOT who
        // the equipment is assigned to/possessed by (that's a separate, not yet
        // implemented concept; see /equipmentassignment). Column name kept as
        // "UserId" so this rename doesn't require a migration.
        [Column("UserId")]
        public string? RegisteredByUserId { get; set; }
        public ApplicationUser? RegisteredByUser { get; set; }

        // Relation 1:N
        public ICollection<EquipmentHistory>? EquipmentHistories { get; set; }
    }
}
