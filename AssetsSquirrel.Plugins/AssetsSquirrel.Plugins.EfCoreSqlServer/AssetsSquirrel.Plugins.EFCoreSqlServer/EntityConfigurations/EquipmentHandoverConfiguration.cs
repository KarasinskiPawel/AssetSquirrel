using AssetSquirrel.CoreBusiness;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.EntityConfigurations
{
    public class EquipmentHandoverConfiguration : IEntityTypeConfiguration<EquipmentHandover>
    {
        public void Configure(EntityTypeBuilder<EquipmentHandover> builder)
        {
            builder.HasKey(e => e.EquipmentHandoverId);

            builder.Property(e => e.HandoverDocumentNumber)
                .IsRequired()
                .HasMaxLength(12);

            builder.Property(e => e.HandoverDate)
                .IsRequired();

            builder.Property(e => e.Comment)
                .HasMaxLength(500);

            builder.HasMany(e => e.EquipmentHandoverDetails)
                .WithOne(d => d.EquipmentHandover)
                .HasForeignKey(e => e.EquipmentHandoverId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.FromLocation)
                .WithMany()
                .HasForeignKey(a => a.FromLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.ToLocation)
                .WithMany()
                .HasForeignKey(a => a.ToLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.FromEmployee)
                .WithMany()
                .HasForeignKey(a => a.FromEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.ToEmployee)
                .WithMany()
                .HasForeignKey(a => a.ToEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
