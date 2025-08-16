using BaitaHora.Domain.Entities.Companies.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations
{
    public sealed class CompanyCustomerProfessionalConfig : IEntityTypeConfiguration<CompanyCustomerProfessional>
    {
        public void Configure(EntityTypeBuilder<CompanyCustomerProfessional> b)
        {
            b.ToTable("company_customer_professionals");
            b.HasKey(x => x.Id);

            b.Property(x => x.CompanyId).IsRequired();
            b.Property(x => x.CustomerId).IsRequired();
            b.Property(x => x.ProfessionalUserId).IsRequired();

            b.Property(x => x.CreatedAtUtc)
             .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

            b.HasIndex(x => new { x.CompanyId, x.CustomerId, x.ProfessionalUserId }).IsUnique();
            b.HasIndex(x => new { x.CompanyId, x.CustomerId, x.IsPrimary });

            b.HasOne(x => x.Company)
             .WithMany()
             .HasForeignKey(x => x.CompanyId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Customer)
             .WithMany() 
             .HasForeignKey(x => x.CustomerId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.ProfessionalUser)
             .WithMany() 
             .HasForeignKey(x => x.ProfessionalUserId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}