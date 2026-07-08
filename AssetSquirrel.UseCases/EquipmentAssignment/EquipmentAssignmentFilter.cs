namespace AssetSquirrel.UseCases.EquipmentAssignment
{
    public class EquipmentAssignmentFilter
    {
        public bool IsActive { get; set; } = true;
        public int? LocationId { get; set; }
        public int? EmployeeId { get; set; }
        public int? ManufacturerId { get; set; }
        public int? HardwareTypeId { get; set; }
        public string? SearchText { get; set; }
    }
}
