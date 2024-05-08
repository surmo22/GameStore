using Microsoft.EntityFrameworkCore;

namespace GameStore.Data
{
    public static class SeedData
    {

        public static async Task EnsurePopulatedAsync(IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices
                        .CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }
            
            if (!await context.Genres.AnyAsync())
            {
                await context.Genres.AddRangeAsync(new Genre { Name = "Action" },
                new Genre { Name = "Adventure" },
                new Genre { Name = "Role-Playing" },
                new Genre { Name = "Strategy" },
                new Genre { Name = "Simulation" },
                new Genre { Name = "Sports" },
                new Genre { Name = "Racing" });
            }

            if (!await context.Games.AnyAsync())
            {
#pragma warning disable S6562 // Always set the "DateTimeKind" when creating new "DateTime" instances
                await context.Games.AddRangeAsync(new Game
                {
                    
                    Title = "The Witcher 3: Wild Hunt",
                    Description = "An action role-playing game set in an open world environment.",
                    Price = 29.99m,
                    ReleaseDate = new DateTime(2015, 5, 19),
                    Publisher = "CD Projekt",
                    Developer = "CD Projekt Red",
                    Platform = "PC, PlayStation 4, Xbox One",
                    CoverImageUrl = "https://upload.wikimedia.org/wikipedia/en/0/0c/Witcher_3_cover_art.jpg",
                    TrailerUrl = "c0i88t0Kacs",
                    GameImages = new List<string>(),
                },
                new Game
                {
                   
                    Title = "Grand Theft Auto V",
                    Description = "An action-adventure game played from either a third-person or first-person perspective.",
                    Price = 39.99m,
                    ReleaseDate = new DateTime(2013, 9, 17),
                    Publisher = "Rockstar Games",
                    Developer = "Rockstar North",
                    Platform = "PC, PlayStation 3, PlayStation 4, Xbox 360, Xbox One",
                    CoverImageUrl = "https://media.rockstargames.com/rockstargames/img/global/news/upload/actual_1364906194.jpg",
                    TrailerUrl = "QkkoHAzjnUs",
                    GameImages = new List<string>(),
                },
                new Game
                {
                    
                    Title = "The Legend of Zelda: Breath of the Wild",
                    Description = "An action-adventure game set in a large open world environment.",
                    Price = 49.99m,
                    ReleaseDate = new DateTime(2017, 3, 3),
                    Publisher = "Nintendo",
                    Developer = "Nintendo EPD",
                    Platform = "Nintendo Switch, Wii U",
                    CoverImageUrl = "https://assetsio.gnwcdn.com/148430785862.jpg?width=1920&height=1920&fit=bounds&quality=80&format=jpg&auto=webp",
                    TrailerUrl = "zw47_q9wbBE",
                    GameImages = new List<string>()
                },
                new Game
                {
                    
                    Title = "Red Dead Redemption 2",
                    Description = "An action-adventure game set in an open world environment.",
                    Price = 59.99m,
                    ReleaseDate = new DateTime(2018, 10, 26),
                    Publisher = "Rockstar Games",
                    Developer = "Rockstar Studios",
                    Platform = "PlayStation 4, Xbox One, PC, Stadia",
                    CoverImageUrl = "https://assets.vg247.com/current//2018/05/red_dead_redemption_2_cover_art_1.jpg",
                    TrailerUrl = "gmA6MrX81z4",
                    GameImages = new List<string>()
                });
#pragma warning restore S6562 // Always set the "DateTimeKind" when creating new "DateTime" instances
            }
            await context.SaveChangesAsync();
        }
    }
}
