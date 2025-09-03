using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Data.Data.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(g => g.Key)
            .IsUnique();

        builder.Property(g => g.Key)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(g => g.Description)
            .HasMaxLength(400);

        builder.Property(g => g.QuantityPerUnit)
            .HasMaxLength(30);

        builder.Property(g => g.ImageUrl)
            .HasMaxLength(400);

        builder.HasMany(g => g.Genres)
            .WithMany(g => g.Games)
            .UsingEntity<GameGenre>(
                gg => gg.HasOne(g => g.Genre)
                    .WithMany()
                    .HasForeignKey(g => g.GenreId)
                    .OnDelete(DeleteBehavior.Restrict),
                gg => gg.HasOne(g => g.Game)
                    .WithMany()
                    .HasForeignKey(g => g.GameId),
                gg =>
                    {
                        gg.HasKey(g => new { g.GameId, g.GenreId });
                        gg.ToTable("GameGenres");
                    });

        builder.HasMany(g => g.Platforms)
            .WithMany(p => p.Games)
            .UsingEntity<GamePlatform>(
                gp => gp.HasOne(p => p.Platform)
                    .WithMany()
                    .HasForeignKey(p => p.PlatformId)
                    .OnDelete(DeleteBehavior.Restrict),
                gp => gp.HasOne(p => p.Game)
                    .WithMany()
                    .HasForeignKey(p => p.GameId),
                gp =>
                {
                    gp.HasKey(p => new { p.GameId, p.PlatformId });
                    gp.ToTable("GamePlatforms");
                });

        builder.HasOne(g => g.Publisher)
            .WithMany(p => p.Games)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
