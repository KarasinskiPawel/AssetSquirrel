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
    public class EquipmentHandoverDetailConfiguration : IEntityTypeConfiguration<EquipmentHandoverDetail>
    {
        public void Configure(EntityTypeBuilder<EquipmentHandoverDetail> builder)
        {
            builder.HasKey(e => e.EquipmentHandoverDetailId);

            builder.HasOne(a => a.EquipmentHandover)
                .WithMany(e => e.EquipmentHandoverDetails)
                .HasForeignKey(e => e.EquipmentHandoverId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.HardwareType)
                .WithMany()
                .HasForeignKey(e => e.HardwareTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(e => e.Comment)
                .HasMaxLength(500);
        }
    }
}
