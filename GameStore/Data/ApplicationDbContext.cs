﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Game> Games => this.Set<Game>();
        public DbSet<Purchase> Purchases => this.Set<Purchase>();
        public DbSet<Genre> Genres => this.Set<Genre>();
        public DbSet<Order> Orders => this.Set<Order>();
        public DbSet<Key> Keys => this.Set<Key>();
        public DbSet<OrderItem> OrderItems => this.Set<OrderItem>();
    }
}
