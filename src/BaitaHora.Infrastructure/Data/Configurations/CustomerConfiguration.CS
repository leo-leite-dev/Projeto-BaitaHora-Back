using BaitaHora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations
{
    public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> b)
        {
            b.ToTable("customers");

            b.HasKey(x => x.Id);

            b.Property(x => x.PhoneE164)
             .IsRequired()
             .HasMaxLength(32);

            b.Property(x => x.Name)
             .HasMaxLength(120);

            b.Property(x => x.IsActive)
             .HasDefaultValue(true);

            b.HasIndex(x => x.PhoneE164).IsUnique();
        }
    }
}