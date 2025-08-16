using BaitaHora.Domain.Entities.Companies.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations
{
    public sealed class CompanyCustomerConfiguration : IEntityTypeConfiguration<CompanyCustomer>
    {
        public void Configure(EntityTypeBuilder<CompanyCustomer> b)
        {
            b.ToTable("company_customers");

            b.HasKey(x => new { x.CompanyId, x.CustomerId });

            b.HasOne(x => x.Company)
             .WithMany()
             .HasForeignKey(x => x.CompanyId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Customer)
             .WithMany()
             .HasForeignKey(x => x.CustomerId)
             .OnDelete(DeleteBehavior.Cascade);

            b.Property(x => x.IsActive).HasDefaultValue(true);

            b.HasIndex(x => x.IsActive);
            b.HasIndex(x => x.PreferredProfessionalUserId);
            b.HasIndex(x => x.LastVisitAtUtc);
        }
    }
}