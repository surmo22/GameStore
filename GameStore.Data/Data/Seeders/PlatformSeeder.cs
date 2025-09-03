using GameStore.Domain.Entities.CoreEntities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data.Data.Seeders;

public static class PlatformSeeder
{
    public static void SeedPlatformData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Platform>()
            .HasData(
                new Platform
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Type = "Mobile",
                },
                new Platform
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Type = "Browser",
                },
                new Platform
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                    Type = "Desktop",
                },
                new Platform
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                    Type = "Console",
                });
    }
}
