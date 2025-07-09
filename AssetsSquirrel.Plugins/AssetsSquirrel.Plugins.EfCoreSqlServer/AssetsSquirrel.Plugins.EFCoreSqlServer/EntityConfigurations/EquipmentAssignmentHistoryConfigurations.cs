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
    internal class EquipmentAssignmentHistoryConfigurations : IEntityTypeConfiguration<EquipmentAssignmentHistory>
    {
        public void Configure(EntityTypeBuilder<EquipmentAssignmentHistory> builder)
        {
            builder.HasKey(eah => eah.EquipmentAssignmentHistoryId);

            builder.HasOne(ea => ea.Equipment)
                   .WithMany()
                   .HasForeignKey(ea => ea.EquipmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ea => ea.Location)
                     .WithMany()
                     .HasForeignKey(ea => ea.LocationId)
                     .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ea => ea.Employee)
                        .WithMany()
                        .HasForeignKey(ea => ea.EmployeeId)
                        .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ea => ea.User)
                     .WithMany()
                     .HasForeignKey(ea => ea.UserId)
                     .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
