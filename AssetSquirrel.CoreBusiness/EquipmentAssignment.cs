using AssetsSquirrel.CoreBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.CoreBusiness
{
    public class EquipmentAssignment
    {
        [Key]
        public int EquipmentAssignmentId { get; set; }
        public int EquipmentId { get; set; }
        public Equipment? Equipment { get; set; }
        public int? LocationId { get; set; }
        public Location? Location { get; set; }
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public DateTime? DateOfHandover { get; set; }
        public DateTime? DateOfReturn { get; set; }

        // Handover document that created this assignment, if any -- lets a
        // document cancellation close exactly the assignments it created.
        public int? EquipmentHandoverId { get; set; }
        public EquipmentHandover? EquipmentHandover { get; set; }

        // Return document that closed this assignment, if any.
        public int? EquipmentReturnId { get; set; }
        public EquipmentReturn? EquipmentReturn { get; set; }

        //Navigation
        public string? UserId { get; set; } // Foreign key to ApplicationUser
        public ApplicationUser? User { get; set; }

        // Relation 1:N
        public ICollection<EquipmentAssignmentHistory>? EquipmentAssignmentHistories { get; set; }
    }
}
