using Lab1_6.Models;
using Lab1_6.Models.Billing;
using Lab1_6.Models.Delivery;
using Lab1_6.Models.Notifications;
using Lab1_6.Models.Orders;
using Lab1_6.Models.Warehouse;
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
        public DbSet<OrderRequest> OrderRequests { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<EmailNotification> EmailNotifications { get; set; }
        public DbSet<WarehouseReservation> WarehouseReservations { get; set; }
        public DbSet<CourierReservation> CourierReservations { get; set; }

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
            modelBuilder.Entity<Account>()
                .Property(a => a.Timestamp)
                .IsRowVersion();

            modelBuilder.Entity<Order>()
                .HasKey(o => o.Id);
            modelBuilder.Entity<Order>()
                .Property(a => a.Timestamp)
                .IsRowVersion();

            modelBuilder.Entity<WarehouseReservation>()
                .HasKey(o => o.OrderId);

            modelBuilder.Entity<CourierReservation>()
                .HasKey(o => o.OrderId);
        }
    }
}
