using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public ApplicationDbContext() : base()
        {

        }
        public virtual DbSet<Game> Games => this.Set<Game>();
        public virtual DbSet<Genre> Genres => this.Set<Genre>();
        public virtual DbSet<Order> Orders => this.Set<Order>();
        public virtual DbSet<Key> Keys => this.Set<Key>();
        public virtual DbSet<OrderItem> OrderItems => this.Set<OrderItem>();
    }
}
