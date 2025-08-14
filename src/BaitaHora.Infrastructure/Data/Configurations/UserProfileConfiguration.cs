using BaitaHora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Persistence.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable("UserProfiles");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.FullName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.CPF)
                   .IsRequired()
                   .HasMaxLength(14);

            builder.Property(p => p.RG)
                   .HasMaxLength(20);

            builder.Property(p => p.Phone)
                   .HasMaxLength(20);

            builder.Property(p => p.ProfileImageUrl)
                   .HasMaxLength(500);

            builder.OwnsOne(p => p.Address, addr =>
            {
                addr.Property(a => a.Street).IsRequired().HasMaxLength(200);
                addr.Property(a => a.Number).IsRequired().HasMaxLength(20);
                addr.Property(a => a.Complement).HasMaxLength(200);
                addr.Property(a => a.Neighborhood).IsRequired().HasMaxLength(100);
                addr.Property(a => a.City).IsRequired().HasMaxLength(100);
                addr.Property(a => a.State).IsRequired().HasMaxLength(2);
                addr.Property(a => a.ZipCode).IsRequired().HasMaxLength(9);

                addr.ToTable("UserProfiles"); 
            });
        }
    }
}