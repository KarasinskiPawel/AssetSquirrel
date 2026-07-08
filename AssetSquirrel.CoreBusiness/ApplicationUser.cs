//using Microsoft.AspNet.Identity.EntityFramework;
using AssetSquirrel.CoreBusiness;
using Microsoft.AspNetCore.Identity;

namespace AssetsSquirrel.CoreBusiness
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string? FirstName { get; set; }
        [PersonalData]
        public string? LastName { get; set; }
        public ICollection<Equipment>? Equipments { get; set; }
        public ICollection<EquipmentHistory>? EquipmentHistories{ get; set; }
        public ICollection<Invoice>? Invoices { get; set; }
    }
}
