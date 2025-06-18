using AssetSquirrel.CoreBusiness;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.EntityConfigurations
{
    internal class EquipmentHistoryConfiguration : IEntityTypeConfiguration<EquipmentHistory>
    {
        public void Configure(EntityTypeBuilder<EquipmentHistory> builder)
        {
            builder.HasKey(eh => eh.EquipmentHistoryId);

            builder.HasOne(eh => eh.Equipment)
                .WithMany(e => e.EquipmentHistories)
                .HasForeignKey(eh => eh.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(eh => eh.Invoice)
                .WithMany()
                .HasForeignKey(eh => eh.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(eh => eh.Suppiler)
                .WithMany()
                .HasForeignKey(eh => eh.SuppilerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(eh => eh.Manufacturer)
                .WithMany()
                .HasForeignKey(eh => eh.ManufacturerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(eh => eh.HardwareType)
                .WithMany()
                .HasForeignKey(eh => eh.HardwareTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
