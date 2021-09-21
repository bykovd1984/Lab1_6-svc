using Lab1_6.Models;
using Lab1_6.Models.Billing;
using Lab1_6.Models.Order;
using Microsoft.EntityFrameworkCore;

namespace Lab1_6.Data
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasKey(u => u.UserName);

            modelBuilder.Entity<Profile>()
                .HasKey(u => u.UserName);

            modelBuilder.Entity<Order>()
                .HasKey(o => o.Id);

            modelBuilder.Entity<OrderRequest>()
                .HasKey(o => o.OrderId);
            modelBuilder.Entity<OrderRequest>()
                .HasOne(o => o.Order)
                .WithOne();
            modelBuilder.Entity<OrderRequest>()
                .HasIndex(o => o.RequestId)
                .IsUnique();

            modelBuilder.Entity<Account>()
                .HasKey(a => a.UserName);

            modelBuilder.Entity<Order>()
                .HasKey(o => o.Id);
        }
    }
}
