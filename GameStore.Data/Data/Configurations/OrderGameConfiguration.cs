using GameStore.Domain.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Data.Data.Configurations;

public class OrderGameConfiguration : IEntityTypeConfiguration<OrderGame>
{
    public void Configure(EntityTypeBuilder<OrderGame> builder)
    {
        builder.HasKey(o => new {o.OrderId, o.ProductId });

        builder.HasIndex(o => o.Id);

        builder.HasOne(o => o.Product);

        builder.HasOne(o => o.Order)
            .WithMany(o => o.Items);

        builder.Property(o => o.Price)
            .IsRequired();

        builder.Property(o => o.Discount)
            .IsRequired();

        builder.Property(o => o.Discount)
            .IsRequired(false);
    }
}