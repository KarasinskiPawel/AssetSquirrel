using AssetSquirrel.CoreBusiness;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.EntityConfigurations
{
    public class EquipmentReturnConfigurations : IEntityTypeConfiguration<EquipmentReturn>
    {
        public void Configure(EntityTypeBuilder<EquipmentReturn> builder)
        {
            builder.HasKey(r => r.EquipmentReturnId);

            builder.Property(r => r.ReturnDocumentNumber)
                .IsRequired()
                .HasMaxLength(14);

            builder.HasIndex(r => r.ReturnDocumentNumber)
                .IsUnique();

            builder.Property(r => r.ReturnDate)
                .IsRequired();

            builder.Property(r => r.Comment)
                .HasMaxLength(500);

            builder.HasOne(r => r.Employee)
                .WithMany()
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Location)
                .WithMany()
                .HasForeignKey(r => r.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.StorageLocation)
                .WithMany()
                .HasForeignKey(r => r.StorageLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.PreparedByUser)
                .WithMany()
                .HasForeignKey(r => r.PreparedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.EquipmentAssignments)
                .WithOne(a => a.EquipmentReturn)
                .HasForeignKey(a => a.EquipmentReturnId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
