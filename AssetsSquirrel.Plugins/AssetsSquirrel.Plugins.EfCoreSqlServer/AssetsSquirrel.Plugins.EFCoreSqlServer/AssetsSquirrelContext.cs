using AssetSquirrel.CoreBusiness;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetsSquirrel.Plugins.EFCoreSqlServer
{
    public class AssetsSquirrelContext : DbContext
    {
        public AssetsSquirrelContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Suppiler> Suppilers { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<HardwareType> HardwareTypes { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<Error> Errors{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee { EmployeeId = 1, FirstName = "Paweł", LastName = "Karasiński", Email = "pawelka@komfort.pl", PhoneNumber = null, IsActive = true },
                new Employee { EmployeeId = 1, FirstName = "Dariusz", LastName = "Gąsiorowski", Email = "dariusz.gasiorowski@komfort.pl", PhoneNumber = null, IsActive = true },
                new Employee { EmployeeId = 1, FirstName = "Konrad", LastName = "Wawrzyniak", Email = "Konrad.Wawrzyniak@komfort.pl", PhoneNumber = null, IsActive = true }
                );

            modelBuilder.Entity<Location>().HasData(
                new Location { LocationId = 1, Code = "M100", MPK = "PL1M100Z", City = "Stryków", Street = "Magazyn Centralny", Email = "", PhoneNumber = "", IsActive = true },
                new Location { LocationId = 2, Code = "S000", MPK = "PL1C001Z", City = "Łódź", Street = "Biuro - Srebrzyńska 14", Email = "", PhoneNumber = "", IsActive = true },
                new Location { LocationId = 3, Code = "N001", MPK = "PL1N001Z", City = "Łódź", Street = "Magazyn IT - Srebrzyńska 14", Email = "", PhoneNumber = "", IsActive = true }
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

            modelBuilder.Entity<Equipment>()
                .HasOne(a => a.Suppiler)
                .WithMany(b => b.Equipments)
                .HasForeignKey(a => a.SuppilerId);

            modelBuilder.Entity<Equipment>()
                .HasOne(a => a.Manufacturer)
                .WithMany(b => b.Equipments)
                .HasForeignKey(a => a.ManufacturerId);

            modelBuilder.Entity<Equipment>()
                .HasOne(a => a.HardwareType)
                .WithMany(b => b.Equipments)
                .HasForeignKey(a => a.HardwareTypeId);
        }
    }
}
