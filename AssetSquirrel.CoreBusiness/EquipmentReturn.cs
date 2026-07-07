using AssetsSquirrel.CoreBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AssetSquirrel.CoreBusiness
{
    public class EquipmentReturn
    {
        [Key]
        public int EquipmentReturnId { get; set; }
        public string ReturnDocumentNumber { get; set; } = string.Empty;

        // Who/where the returned equipment was assigned to before the return.
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        public DateTime ReturnDate { get; set; } = DateTime.Now;
        public string? Comment { get; set; }

        // Warehouse location (Location.EquipmentStorage) the returned
        // equipment is physically placed into.
        public int StorageLocationId { get; set; }
        public Location? StorageLocation { get; set; }

        public string? PreparedByUserId { get; set; }
        public ApplicationUser? PreparedByUser { get; set; }

        public string? FilePath { get; set; }
        public DateTime? UploadDate { get; set; }

        public ICollection<EquipmentAssignment> EquipmentAssignments { get; set; } = new List<EquipmentAssignment>();
    }
}
