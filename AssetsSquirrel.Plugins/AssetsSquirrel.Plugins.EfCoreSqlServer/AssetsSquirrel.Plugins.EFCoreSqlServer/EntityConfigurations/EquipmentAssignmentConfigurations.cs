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
    internal class EquipmentAssignmentConfigurations : IEntityTypeConfiguration<EquipmentAssignment>
    {
        public void Configure(EntityTypeBuilder<EquipmentAssignment> builder)
        {
            builder.HasKey(ea => ea.EquipmentAssignmentId);

            //builder.HasOne(ea => ea.Equipment)
            //       .WithMany(e => e.EquipmentAssignments)
            //       .HasForeignKey(ea => ea.EquipmentId)
            //       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
