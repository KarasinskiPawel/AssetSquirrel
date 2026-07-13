namespace AssetSquirrelAuthorize.WebApp.Components.ViewModels
{
    public class EquipmentRequest
    {
        public string? SearchText { get; set; }
        public bool IsActive { get; set; } = true;
        public int? SuppilerId { get; set; }
        public int? ManufacturerId { get; set; }
        public int? HardwareTypeId { get; set; }
    }
}
