using BaitaHora.Domain.Entities.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Persistence.Configurations
{
    public class CompanyImageConfiguration : IEntityTypeConfiguration<CompanyImage>
    {
        public void Configure(EntityTypeBuilder<CompanyImage> builder)
        {
            builder.ToTable("CompanyImages");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Url)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(i => i.UploadedAt)
                   .IsRequired();

            builder.HasOne(i => i.Company)
                   .WithOne(c => c.Image)
                   .HasForeignKey<CompanyImage>(i => i.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}