using System;

namespace AssetSquirrel.CoreBusiness.Dto
{
    public class EquipmentAssignmentOverviewDto
    {
        public int EquipmentId { get; set; }
        public string? SuppilerName { get; set; }
        public int? ManufacturerId { get; set; }
        public string? ManufacturerName { get; set; }
        public int? HardwareTypeId { get; set; }
        public string? HardwareTypeName { get; set; }
        public string? ModelName { get; set; }
        public string? SerialNumber { get; set; }
        public string? InventoryNumber { get; set; }
        public string? InvoiceNumber { get; set; }
        public bool IsActive { get; set; }

        public int? AssignedEmployeeId { get; set; }
        public string? AssignedEmployeeName { get; set; }
        public int? AssignedLocationId { get; set; }
        public string? AssignedLocationName { get; set; }
        public DateTime? DateOfHandover { get; set; }

        public bool IsAssigned => AssignedEmployeeId is not null || AssignedLocationId is not null;
    }
}
