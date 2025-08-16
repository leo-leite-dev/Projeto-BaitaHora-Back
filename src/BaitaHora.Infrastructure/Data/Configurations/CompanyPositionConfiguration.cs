using BaitaHora.Domain.Entities.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class CompanyPositionConfiguration : IEntityTypeConfiguration<CompanyPosition>
{
    public void Configure(EntityTypeBuilder<CompanyPosition> builder)
    {
        builder.ToTable("CompanyPositions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(80)
               .IsRequired();

        builder.Property(x => x.AccessLevel)
               .HasConversion<short>() 
               .IsRequired();

        builder.Property(x => x.IsActive)
               .HasDefaultValue(true);

        // RELACIONAMENTO CORRETO (apenas UM)
        builder.HasOne(x => x.Company)
               .WithMany(c => c.Positions)       
               .HasForeignKey(x => x.CompanyId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
