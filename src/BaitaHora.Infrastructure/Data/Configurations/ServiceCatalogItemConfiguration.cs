using BaitaHora.Domain.Entities.Companies;
using BaitaHora.Domain.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations
{
    public sealed class ServiceCatalogItemConfiguration : IEntityTypeConfiguration<ServiceCatalogItem>
    {
        public void Configure(EntityTypeBuilder<ServiceCatalogItem> b)
        {
            b.ToTable("service_catalog_items");

            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
             .IsRequired()
             .HasMaxLength(120);

            b.Property(x => x.DurationMinutes)
             .IsRequired();

            b.Property(x => x.Price)
             .HasPrecision(12, 2); 

            b.Property(x => x.IsActive)
             .IsRequired();

            b.HasIndex(x => new { x.CompanyId, x.Name })
             .IsUnique();

            b.HasOne<Company>()
             .WithMany()
             .HasForeignKey(x => x.CompanyId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}