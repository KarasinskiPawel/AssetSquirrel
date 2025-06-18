using AssetSquirrel.CoreBusiness;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.EntityConfigurations
{
    internal class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
    {
        public void Configure(EntityTypeBuilder<Equipment> builder)
        {
            builder.HasKey(e => e.EquipmentId);

            builder.HasOne(e => e.Suppiler)
                   .WithMany(s => s.Equipments)
                   .HasForeignKey(e => e.SuppilerId);

            builder.HasOne(e => e.Manufacturer)
                   .WithMany(m => m.Equipments)
                   .HasForeignKey(e => e.ManufacturerId);

            builder.HasOne(e => e.HardwareType)
                   .WithMany(ht => ht.Equipments)
                   .HasForeignKey(e => e.HardwareTypeId);

            builder.HasOne(e => e.Invoice)
                   .WithMany(i => i.Equipments)
                   .HasForeignKey(e => e.InvoiceId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
