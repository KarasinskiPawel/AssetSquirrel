using AssetsSquirrel.CoreBusiness;
using System;
using System.Collections.Generic;

namespace AssetSquirrel.CoreBusiness.Dto
{
    public class EquipmentReturnDto
    {
        public int EquipmentReturnId { get; set; }
        public string ReturnDocumentNumber { get; set; } = string.Empty;

        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        public DateTime ReturnDate { get; set; } = DateTime.Now;
        public string? Comment { get; set; }

        public int StorageLocationId { get; set; }
        public string? StorageLocationName { get; set; }

        public string? PreparedByUserId { get; set; }
        public string? PreparedByUserName { get; set; }

        public string? FilePath { get; set; }
        public DateTime? UploadDate { get; set; }

        public List<EquipmentAssignmentDto> Items { get; set; } = new List<EquipmentAssignmentDto>();
    }
}
