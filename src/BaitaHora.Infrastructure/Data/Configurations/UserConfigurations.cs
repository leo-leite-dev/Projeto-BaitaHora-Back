using BaitaHora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Username)
                   .HasMaxLength(20);

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(u => u.PasswordHash)
                   .IsRequired();

            builder.Property(u => u.IsActive)
                   .IsRequired();

            builder.Property(u => u.CreatedAt)
                   .IsRequired();
            builder.Property(u => u.UpdatedAt);

            builder.Property(u => u.PasswordResetToken)
                   .HasMaxLength(200);
            builder.Property(u => u.PasswordResetTokenExpiresAt);

            builder.HasOne(u => u.Profile)
                   .WithOne()
                   .HasForeignKey<User>(u => u.ProfileId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<CompanyMember>()
                   .WithOne(cm => cm.User)
                   .HasForeignKey(cm => cm.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}