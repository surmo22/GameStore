using GameStore.Domain.Entities.CoreEntities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data.Data.Seeders;

public static class GenreSeeder
{
    public static void SeedGenreData(ModelBuilder modelBuilder)
    {
        var strategyId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var sportId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var racesId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        var actionId = Guid.Parse("00000000-0000-0000-0000-000000000004");

        modelBuilder.Entity<Genre>()
            .HasData(
                new Genre
                {
                    Id = strategyId,
                    Name = "Strategy",
                    ParentGenreId = null,
                },
                new Genre
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                    Name = "RTS",
                    ParentGenreId = strategyId,
                },
                new Genre
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000006"),
                    Name = "TBS",
                    ParentGenreId = strategyId,
                },
                new Genre
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000007"),
                    Name = "RPG",
                    ParentGenreId = null,
                },
                new Genre
                {
                    Id = sportId,
                    Name = "Sports",
                    ParentGenreId = null,
                },
                new Genre
                {
                    Id = racesId,
                    Name = "Races",
                    ParentGenreId = sportId,
                },
                new Genre
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000008"),
                    Name = "Rally",
                    ParentGenreId = racesId,
                },
                new Genre
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000009"),
                    Name = "Arcade",
                    ParentGenreId = racesId,
                },
                new Genre
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000010"),
                    Name = "Formula",
                    ParentGenreId = racesId,
                },
                new Genre
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000011"),
                    Name = "Off-road",
                    ParentGenreId = racesId,
                },
                new Genre
                {
                    Id = actionId,
                    Name = "Action",
                    ParentGenreId = null,
                },
                new Genre
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000012"),
                    Name = "FPS",
                    ParentGenreId = actionId,
                },
                new Genre
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000013"),
                    Name = "TPS",
                    ParentGenreId = actionId,
                });
    }
}
