using BaitaHora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Persistence.Configurations
{
    public class CompanyMemberConfiguration : IEntityTypeConfiguration<CompanyMember>
    {
        public void Configure(EntityTypeBuilder<CompanyMember> builder)
        {
            builder.ToTable("CompanyMembers");

            builder.HasKey(m => new { m.CompanyId, m.UserId });

            builder.Property(m => m.Role)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(m => m.JoinedAt)
                   .IsRequired();

            builder.Property(m => m.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.HasIndex(m => new { m.CompanyId, m.UserId, m.IsActive });

            builder.HasOne(m => m.Company)
                   .WithMany(c => c.Members)
                   .HasForeignKey(m => m.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.User)
                   .WithMany()
                   .HasForeignKey(m => m.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}