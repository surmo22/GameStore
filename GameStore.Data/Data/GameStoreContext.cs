using GameStore.Data.Data.Configurations;
using GameStore.Data.Data.Seeders;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.Relations;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data.Data;

public class GameStoreContext(DbContextOptions<GameStoreContext> options) : IdentityDbContext<User, Role, Guid>(options)
{
    public DbSet<Game> Games { get; set; }

    public DbSet<Genre> Genres { get; set; }

    public DbSet<Platform> Platforms { get; set; }

    public DbSet<Publisher> Publishers { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<UserBan> UserBans { get; set; }
    
    public DbSet<OrderGame> OrderGame { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new GenreConfiguration());
        builder.ApplyConfiguration(new GameGenreConfiguration());
        builder.ApplyConfiguration(new GamePlatformConfiguration());
        builder.ApplyConfiguration(new PlatformConfiguration());
        builder.ApplyConfiguration(new GameConfiguration());
        builder.ApplyConfiguration(new PublisherConfiguration());
        builder.ApplyConfiguration(new OrderConfiguration());
        builder.ApplyConfiguration(new OrderGameConfiguration());
        builder.ApplyConfiguration(new PaymentMethodConfiguration());
        builder.ApplyConfiguration(new CommentConfiguration());
        builder.ApplyConfiguration(new UserBanConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new RoleConfiguration());

        GenreSeeder.SeedGenreData(builder);
        PlatformSeeder.SeedPlatformData(builder);
        UserAndRoleSeeder.SeedUsersAndRolesData(builder);
    }
}