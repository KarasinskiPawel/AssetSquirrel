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

            builder.HasMany(ea => ea.EquipmentAssignmentHistories)
               .WithOne(eah => eah.EquipmentAssignment)
               .HasForeignKey(eah => eah.EquipmentAssignmentId)
               .OnDelete(DeleteBehavior.Restrict);

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

            builder.HasOne(ea => ea.EquipmentHandover)
                     .WithMany()
                     .HasForeignKey(ea => ea.EquipmentHandoverId)
                     .OnDelete(DeleteBehavior.Restrict);

            // At most one open (not-yet-returned) assignment per equipment
            // unit at a time -- the DB-level guarantee against double
            // handover, independent of any application-layer check.
            builder.HasIndex(ea => ea.EquipmentId)
                     .IsUnique()
                     .HasFilter("[DateOfReturn] IS NULL");
        }
    }
}
