using BaitaHora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Configurations
{
       public class CompanyConfiguration : IEntityTypeConfiguration<Company>
       {
              public void Configure(EntityTypeBuilder<Company> builder)
              {
                     builder.ToTable("Companies");

                     builder.HasKey(c => c.Id);
                     builder.Property(c => c.Id).ValueGeneratedNever();

                     builder.Property(c => c.Name)
                            .IsRequired()
                            .HasMaxLength(200);

                     builder.HasIndex(c => c.Name).IsUnique();
                     builder.HasIndex(c => c.Name);

                     builder.Property(c => c.Document)
                            .HasMaxLength(18);
                     builder.HasIndex(c => c.Document).IsUnique();

                     builder.Property(c => c.IsActive)
                            .IsRequired()
                            .HasDefaultValue(true);

                     builder.Property(c => c.CreatedAtUtc)
                            .IsRequired()
                            .HasDefaultValueSql("timezone('utc', now())");

                     builder.OwnsOne(c => c.Address, addr =>
                     {
                            addr.Property(a => a.Street).IsRequired().HasMaxLength(200).HasColumnName("Address_Street");
                            addr.Property(a => a.Number).IsRequired().HasMaxLength(20).HasColumnName("Address_Number");
                            addr.Property(a => a.Neighborhood).IsRequired().HasMaxLength(100).HasColumnName("Address_Neighborhood");
                            addr.Property(a => a.City).IsRequired().HasMaxLength(100).HasColumnName("Address_City");
                            addr.Property(a => a.State).IsRequired().HasMaxLength(2).HasColumnType("char(2)").HasColumnName("Address_State");
                            addr.Property(a => a.ZipCode).IsRequired().HasMaxLength(9).HasColumnName("Address_ZipCode");
                            addr.Property(a => a.Complement).HasMaxLength(200).HasColumnName("Address_Complement");
                     });

                     builder.HasMany(c => c.Members)
                            .WithOne(m => m.Company)
                            .HasForeignKey(m => m.CompanyId)
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasMany(c => c.Positions)
                            .WithOne(p => p.Company)
                            .HasForeignKey(p => p.CompanyId)
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasOne(c => c.Image)
                            .WithOne(i => i.Company)
                            .HasForeignKey<CompanyImage>(i => i.CompanyId)
                            .IsRequired(false)
                            .OnDelete(DeleteBehavior.Cascade);
              }
       }
}