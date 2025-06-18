using AssetSquirrel.CoreBusiness;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.EntityConfigurations
{
    internal class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(i => i.InvoiceId);

            builder.Property(i => i.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(i => i.Description)
                .HasMaxLength(200);

            builder.HasOne(i => i.User)
                .WithMany(u => u.Invoices)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
