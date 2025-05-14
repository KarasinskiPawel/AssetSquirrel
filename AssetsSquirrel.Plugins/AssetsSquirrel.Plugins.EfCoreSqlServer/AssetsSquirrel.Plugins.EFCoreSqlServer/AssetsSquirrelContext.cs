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
        public DbSet<DictionaryEquipment>? DictionaryEquipments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DictionaryEquipment>().HasData(
                new DictionaryEquipment { DictionaryEquipmentId = 1, Name = "AccessPoint", Description = "", IsActive=true },
                new DictionaryEquipment { DictionaryEquipmentId = 2, Name = "Drukarka etykiet", Description = "", IsActive = true },
                new DictionaryEquipment { DictionaryEquipmentId = 3, Name = "Drukarka fiskalna", Description = "", IsActive = true },
                new DictionaryEquipment { DictionaryEquipmentId = 4, Name = "Dysk zewnętrzny", Description = "", IsActive = true }
                );
        }
    }
}
