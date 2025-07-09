using AssetsSquirrel.CoreBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.CoreBusiness
{
    public class EquipmentAssignmentHistory
    {
        [Key]
        public int EquipmentAssignmentHistoryId { get; set; }
        public int EquipmentAssignmentId { get; set; }
        [Required]
        public EquipmentAssignment EquipmentAssignment { get; set; }
        public int EquipmentId { get; set; }
        public Equipment? Equipment { get; set; }
        public int? LocationId { get; set; }
        public Location? Location { get; set; }
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public DateTime? DateOfHandover { get; set; }
        public DateTime? DateOfReturn { get; set; }
        public DateTime ChangeDate { get; set; } = DateTime.Now; // When the change was made

        //Navigation
        public string? UserId { get; set; } // Foreign key to ApplicationUser
        public ApplicationUser? User { get; set; }
    }
}
