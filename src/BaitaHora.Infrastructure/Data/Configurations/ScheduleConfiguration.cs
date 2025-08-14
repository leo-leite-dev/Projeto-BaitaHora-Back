using BaitaHora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations
{
    public sealed class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> b)
        {
            b.ToTable("schedules");

            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.UserId, x.CompanyId })
             .IsUnique();

            b.Property(x => x.IsActive)
             .IsRequired();

            b.Property(x => x.CreatedAtUtc)
             .IsRequired()
             .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

            b.HasOne(x => x.User)
             .WithMany()
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Company)
             .WithMany()
             .HasForeignKey(x => x.CompanyId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
