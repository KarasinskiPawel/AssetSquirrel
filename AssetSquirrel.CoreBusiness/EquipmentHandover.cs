using AssetsSquirrel.CoreBusiness;
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
        // Issuing side is not modeled via FromLocation/FromEmployee -- see
        // PreparedByUserId below, which is the field actually populated/shown.
        public int? FromLocationId { get; set; }
        public Location? FromLocation { get; set; }
        public int? ToLocationId { get; set; }
        public Location? ToLocation { get; set; }
        public int? FromEmployeeId { get; set; }
        public Employee? FromEmployee { get; set; }
        public int? ToEmployeeId { get; set; }
        public Employee? ToEmployee { get; set; }
        public DateTime HandoverDate { get; set; }
        public string? Comment { get; set; }
        // True once a signed scan of the printed document has been attached
        // (see EquipmentHandoverAddDocumentDialogBox), not merely on save.
        public bool IsPosted { get; set; }
        public bool IsActive { get; set; } = true;

        // Person who prepared/issued this handover -- the logged-in user,
        // set in code, never editable in the UI.
        public string? PreparedByUserId { get; set; }
        public ApplicationUser? PreparedByUser { get; set; }

        public string? FilePath { get; set; }
        public DateTime? UploadDate { get; set; }

        public ICollection<EquipmentHandoverDetail> EquipmentHandoverDetails { get; set; } = new List<EquipmentHandoverDetail>();

    }
}
