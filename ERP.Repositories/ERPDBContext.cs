using ERP.Domain.Enums;
using ERP.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Repositories
{
    public class ERPDBContext : IdentityDbContext<ApplicationUser>
    {
        public ERPDBContext(DbContextOptions<ERPDBContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItem { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<InventoryLog> InventoryLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ── Fluent config
            builder.Entity<Order>()
                .Ignore(o => o.RemainingAmount);

            builder.Entity<OrderItem>()
                .Ignore(oi => oi.TotalPrice);

            builder.Entity<PurchaseItem>()
                .Ignore(pi => pi.TotalCost);

            builder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>();

            builder.Entity<Order>()
                .Property(o => o.PaymentStatus)
                .HasConversion<string>();

            builder.Entity<Purchase>()
                .Property(p => p.Status)
                .HasConversion<string>();

            builder.Entity<Payment>()
                .Property(p => p.PaymentMethod)
                .HasConversion<string>();

            builder.Entity<InventoryLog>()
                .Property(il => il.Type)
                .HasConversion<string>();

            // ── Relationship: Order → Customer 
            builder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);

            // ── Relationship: Payment → Order 
            builder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId);

            // ── Relationship: Order → ApplicationUser 
            builder.Entity<Order>()
                .HasOne(o => o.CreatedByUser)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.CreatedByUserId);

            // ── Relationship: Purchase → ApplicationUser 
            builder.Entity<Purchase>()
                .HasOne(p => p.CreatedByUser)
                .WithMany(u => u.Purchases)
                .HasForeignKey(p => p.CreatedByUserId);

            // ── Relationship: InventoryLog → ApplicationUser 
            builder.Entity<InventoryLog>()
                .HasOne(il => il.CreatedByUser)
                .WithMany(u => u.InventoryLogs)
                .HasForeignKey(il => il.CreatedByUserId);

            // ═════════════════════════════════════════════════════════════════
            //  DATA SEEDING
            // ═════════════════════════════════════════════════════════════════

            SeedRoles(builder);
            SeedUsers(builder);

        }

        // =====================================================================
        // ROLES
        // =====================================================================
        private static void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "ROLE-ADMIN-0001",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "ROLE-ADMIN-STAMP"
                },
                new IdentityRole
                {
                    Id = "ROLE-SALES-0001",
                    Name = "SalesEmployee",
                    NormalizedName = "SALESEMPLOYEE",
                    ConcurrencyStamp = "ROLE-SALES-STAMP"
                },
                new IdentityRole
                {
                    Id = "ROLE-WARE-0001",
                    Name = "WarehouseEmployee",
                    NormalizedName = "WAREHOUSEEMPLOYEE",
                    ConcurrencyStamp = "ROLE-WARE-STAMP"
                }
            );
        }

        // =====================================================================
        // USERS  (1 Admin · 1 SalesEmployee · 1 WarehouseEmployee)
        // =====================================================================
        private static void SeedUsers(ModelBuilder builder)
        {
            var hasher = new PasswordHasher<ApplicationUser>();

            // ── Admin 
            var admin = new ApplicationUser
            {
                Id = "USER-ADMIN-0001",
                UserName = "admin@erp.com",
                NormalizedUserName = "ADMIN@ERP.COM",
                Email = "admin@erp.com",
                NormalizedEmail = "ADMIN@ERP.COM",
                EmailConfirmed = true,
                FullName = "System Administrator",
                JobTitle = "System Admin",
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                SecurityStamp = "ADMIN-SECURITY-STAMP",
                ConcurrencyStamp = "ADMIN-CONCURRENCY-STAMP",
                PasswordHash = "AQAAAAIAAYagAAAAEFUfno0apiyLD/H8ltTclkliznkIZH6rI8yl1YGS1dU1uThxbTPavE4cAWJGQzVzQA=="
            };
            //admin.PasswordHash = hasher.HashPassword(admin, "Admin@123456");

            // ── Sales Employee 
            var salesUser = new ApplicationUser
            {
                Id = "USER-SALES-0001",
                UserName = "sarah.sales@erp.com",
                NormalizedUserName = "SARAH.SALES@ERP.COM",
                Email = "sarah.sales@erp.com",
                NormalizedEmail = "SARAH.SALES@ERP.COM",
                EmailConfirmed = true,
                FullName = "Sarah Johnson",
                JobTitle = "Sales Representative",
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                SecurityStamp = "SALES-SECURITY-STAMP",
                ConcurrencyStamp = "SALES-CONCURRENCY-STAMP",
                PasswordHash = "AQAAAAIAAYagAAAAEAVnV/2QHL/X98hyikRM3G8yaVl6aDRp4DQgPcZklWl+0fSaDqUpfxYwIhC9Ru75Nw=="
            };
            //salesUser.PasswordHash = hasher.HashPassword(salesUser, "Sales@123456");

            // ── Warehouse Employee 
            var warehouseUser = new ApplicationUser
            {
                Id = "USER-WARE-0001",
                UserName = "mike.warehouse@erp.com",
                NormalizedUserName = "MIKE.WAREHOUSE@ERP.COM",
                Email = "mike.warehouse@erp.com",
                NormalizedEmail = "MIKE.WAREHOUSE@ERP.COM",
                EmailConfirmed = true,
                FullName = "Mike Thompson",
                JobTitle = "Warehouse Manager",
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                SecurityStamp = "WARE-SECURITY-STAMP",
                ConcurrencyStamp = "WARE-CONCURRENCY-STAMP",
                PasswordHash = "AQAAAAIAAYagAAAAEFyCBJ7rkekCduH/qr+5QFx4ffatRF1x6/ci9RnMQMnfVdDqhe7z0PoAJtiWIYarhQ=="
            };
            //warehouseUser.PasswordHash = hasher.HashPassword(warehouseUser, "Warehouse@123456");

            builder.Entity<ApplicationUser>().HasData(admin, salesUser, warehouseUser);

            // ── User → Role mapping 
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = "USER-ADMIN-0001", RoleId = "ROLE-ADMIN-0001" },
                new IdentityUserRole<string> { UserId = "USER-SALES-0001", RoleId = "ROLE-SALES-0001" },
                new IdentityUserRole<string> { UserId = "USER-WARE-0001", RoleId = "ROLE-WARE-0001" }
            );
        }

    }
}
