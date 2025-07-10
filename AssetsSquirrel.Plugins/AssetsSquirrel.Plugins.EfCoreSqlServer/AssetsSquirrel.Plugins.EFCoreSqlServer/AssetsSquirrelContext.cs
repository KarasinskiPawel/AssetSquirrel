using AssetSquirrel.CoreBusiness;
using AssetsSquirrel.CoreBusiness;
using AssetsSquirrel.Plugins.EFCoreSqlServer.EntityConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AssetsSquirrel.Plugins.EFCoreSqlServer
{
    public class AssetsSquirrelContext(DbContextOptions<AssetsSquirrelContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Suppiler> Suppilers { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<HardwareType> HardwareTypes { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EquipmentHistory> EquipmentHistories { get; set; }

        public DbSet<EquipmentAssignment> EquipmentAssignments { get; set; }
        public DbSet<EquipmentAssignmentHistory> EquipmentAssignmentHistories { get; set; }
        public DbSet<EquipmentHandover> EquipmentHandovers { get; set; }
        public DbSet<EquipmentHandoverDetail> EquipmentHandoverDetails { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Error> Errors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>().HasData(
                new Employee { EmployeeId = 1, FirstName = "Pawe³", LastName = "Karasiñski", Email = "pawelka@komfort.pl", PhoneNumber = null, IsActive = true },
                new Employee { EmployeeId = 2, FirstName = "Dariusz", LastName = "G¹siorowski", Email = "dariusz.gasiorowski@komfort.pl", PhoneNumber = null, IsActive = true },
                new Employee { EmployeeId = 3, FirstName = "Konrad", LastName = "Wawrzyniak", Email = "Konrad.Wawrzyniak@komfort.pl", PhoneNumber = null, IsActive = true }
                );

            modelBuilder.Entity<Location>().HasData(
                new Location { LocationId = 1, Code = "M100", MPK = "PL1M100Z", City = "Stryków", Street = "Magazyn Centralny", Email = "", PhoneNumber = "", IsActive = true },
                new Location { LocationId = 2, Code = "S000", MPK = "PL1C001Z", City = "£ódŸ", Street = "Biuro - Srebrzyñska 14", Email = "", PhoneNumber = "", IsActive = true },
                new Location { LocationId = 3, Code = "N001", MPK = "PL1N001Z", City = "£ódŸ", Street = "Magazyn IT - Srebrzyñska 14", Email = "", PhoneNumber = "", IsActive = true }
                );

            modelBuilder.Entity<Suppiler>().HasData(
                new Suppiler { SuppilerId = 1, Name = "X-KOM", Description = "", IsActive = true },
                new Suppiler { SuppilerId = 2, Name = "MPC", Description = "", IsActive = true },
                new Suppiler { SuppilerId = 3, Name = "Lantre", Description = "", IsActive = true }
                );

            modelBuilder.Entity<Manufacturer>().HasData(
                new Manufacturer { ManufacturerId = 1, Name = "Asus", Description = "", IsActive = true },
                new Manufacturer { ManufacturerId = 2, Name = "Acer", Description = "", IsActive = true },
                new Manufacturer { ManufacturerId = 3, Name = "HP", Description = "", IsActive = true },
                new Manufacturer { ManufacturerId = 4, Name = "Lenovo", Description = "", IsActive = true },
                new Manufacturer { ManufacturerId = 5, Name = "Cisco", Description = "", IsActive = true }
                );

            modelBuilder.Entity<HardwareType>().HasData(
                new HardwareType { HardwareTypeId = 1, Name = "Komputer", Description = "", IsActive = true },
                new HardwareType { HardwareTypeId = 2, Name = "Laptop", Description = "", IsActive = true },
                new HardwareType { HardwareTypeId = 3, Name = "Monitor", Description = "", IsActive = true },
                new HardwareType { HardwareTypeId = 4, Name = "Mysz", Description = "", IsActive = true },
                new HardwareType { HardwareTypeId = 5, Name = "Mysz optyczna", Description = "", IsActive = true },
                new HardwareType { HardwareTypeId = 6, Name = "Klawiatura", Description = "", IsActive = true },
                new HardwareType { HardwareTypeId = 7, Name = "Drukarka", Description = "", IsActive = true },
                new HardwareType { HardwareTypeId = 8, Name = "Drukarka fiskalna", Description = "", IsActive = true }
                );

            modelBuilder.ApplyConfiguration(new EquipmentConfiguration());
            modelBuilder.ApplyConfiguration(new EquipmentHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
            modelBuilder.ApplyConfiguration(new EquipmentAssignmentConfigurations());
            modelBuilder.ApplyConfiguration(new EquipmentAssignmentHistoryConfigurations());
            modelBuilder.ApplyConfiguration(new EquipmentHandoverConfiguration());
            modelBuilder.ApplyConfiguration(new EquipmentHandoverDetailConfiguration());
        }
    }
}
