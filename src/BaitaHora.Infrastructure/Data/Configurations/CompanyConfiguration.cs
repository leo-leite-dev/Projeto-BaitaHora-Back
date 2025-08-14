using BaitaHora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(c => c.Document)
                   .HasMaxLength(32);

            builder.Property(c => c.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(c => c.CreatedAt)
                   .IsRequired();

            builder.OwnsOne(c => c.Address, addr =>
            {
                addr.Property(a => a.Street).IsRequired().HasMaxLength(200);
                addr.Property(a => a.Number).IsRequired().HasMaxLength(20);
                addr.Property(a => a.Neighborhood).IsRequired().HasMaxLength(100);
                addr.Property(a => a.City).IsRequired().HasMaxLength(100);
                addr.Property(a => a.State).IsRequired().HasMaxLength(2);
                addr.Property(a => a.ZipCode).IsRequired().HasMaxLength(9);
                addr.Property(a => a.Complement).HasMaxLength(200);
            });

            builder.HasMany(c => c.Members)
                   .WithOne(m => m.Company)
                   .HasForeignKey(m => m.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Image)
                   .WithOne(i => i.Company)
                   .HasForeignKey<CompanyImage>(i => i.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}