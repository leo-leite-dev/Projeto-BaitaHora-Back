using BaitaHora.Domain.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations
{
    public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> b)
        {
            b.ToTable("appointments");
            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.ScheduleId, x.StartsAtUtc });
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.ServiceId);

            b.Property(x => x.StartsAtUtc).IsRequired();
            b.Property(x => x.EndsAtUtc).IsRequired();
            b.Property(x => x.Status).HasConversion<int>().IsRequired();
            b.Property(x => x.CreatedBy).HasConversion<int>().IsRequired();

            b.Property(x => x.Notes).HasMaxLength(1024);
            b.Property(x => x.CustomerDisplayName).HasMaxLength(120);
            b.Property(x => x.CustomerPhone).HasMaxLength(40);

            b.HasOne(x => x.Schedule)
             .WithMany(s => s.Appointments)
             .HasForeignKey(x => x.ScheduleId)
             .OnDelete(DeleteBehavior.Cascade);

            // trocado de User -> Customer
            b.HasOne(x => x.Customer)
             .WithMany()
             .HasForeignKey(x => x.CustomerId)
             .OnDelete(DeleteBehavior.SetNull);

            b.HasCheckConstraint("CK_appointments_time_range", "\"EndsAtUtc\" > \"StartsAtUtc\"");
        }
    }
}