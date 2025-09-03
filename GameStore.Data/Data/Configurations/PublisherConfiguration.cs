using GameStore.Domain.Entities.CoreEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Data.Data.Configurations;

public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
{
    public void Configure(EntityTypeBuilder<Publisher> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.CompanyName)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(p => p.CompanyName)
            .IsUnique();

        builder.Property(p => p.HomePage).HasMaxLength(400);

        builder.Property(p => p.Description).HasMaxLength(400);
    }
}