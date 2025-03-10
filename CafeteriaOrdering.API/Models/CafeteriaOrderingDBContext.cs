using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CafeteriaOrdering.API.Models
{
    public partial class CafeteriaOrderingDBContext : DbContext
    {
        public CafeteriaOrderingDBContext()
        {
        }

        public CafeteriaOrderingDBContext(DbContextOptions<CafeteriaOrderingDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccountActivity> AccountActivities { get; set; } = null!;
        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<Delivery> Deliveries { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<Menu> Menus { get; set; } = null!;
        public virtual DbSet<MenuItem> MenuItems { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderItem> OrderItems { get; set; } = null!;
        public virtual DbSet<RecommendedMeal> RecommendedMeals { get; set; } = null!;
        public virtual DbSet<RevenueReport> RevenueReports { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

<<<<<<< HEAD
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=LAPTOP-A6P8DBMT\\SQLEXPRESS;Database=CafeteriaOrderingDB;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

=======
>>>>>>> main
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountActivity>(entity =>
            {
                entity.HasKey(e => e.ActivityId)
<<<<<<< HEAD
                    .HasName("PK__account___482FBD63255EB588");
=======
                    .HasName("PK__account___482FBD63F92214AA");
>>>>>>> main

                entity.ToTable("account_activity");

                entity.Property(e => e.ActivityId).HasColumnName("activity_id");

                entity.Property(e => e.ActivityTime)
                    .HasColumnType("datetime")
                    .HasColumnName("activity_time")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ActivityType)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("activity_type");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AccountActivities)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_account_activity_users");
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("addresses");

                entity.Property(e => e.AddressId).HasColumnName("address_id");

                entity.Property(e => e.AddressLine)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("address_line");

                entity.Property(e => e.City)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDefault).HasColumnName("is_default");

                entity.Property(e => e.State)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("state");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.ZipCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("zip_code");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_addresses_users");
            });

            modelBuilder.Entity<Delivery>(entity =>
            {
                entity.ToTable("delivery");

                entity.Property(e => e.DeliveryId).HasColumnName("delivery_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeliverUserId).HasColumnName("deliver_user_id");

                entity.Property(e => e.DeliveryStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("delivery_status");

                entity.Property(e => e.DeliveryTime)
                    .HasColumnType("datetime")
                    .HasColumnName("delivery_time");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.PickupTime)
                    .HasColumnType("datetime")
                    .HasColumnName("pickup_time");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.DeliverUser)
                    .WithMany(p => p.Deliveries)
                    .HasForeignKey(d => d.DeliverUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_delivery_users");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Deliveries)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_delivery_orders");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("feedback");

                entity.Property(e => e.FeedbackId).HasColumnName("feedback_id");

                entity.Property(e => e.Comment)
                    .IsUnicode(false)
                    .HasColumnName("comment");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_feedback_orders");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_feedback_users");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("menus");

                entity.Property(e => e.MenuId).HasColumnName("menu_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.ManagerId).HasColumnName("manager_id");

                entity.Property(e => e.MenuName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("menu_name");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Manager)
                    .WithMany(p => p.Menus)
                    .HasForeignKey(d => d.ManagerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_menus_users");
            });

            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.HasKey(e => e.ItemId)
<<<<<<< HEAD
                    .HasName("PK__menu_ite__52020FDD25A390D3");
=======
                    .HasName("PK__menu_ite__52020FDD902DC51A");
>>>>>>> main

                entity.ToTable("menu_items");

                entity.Property(e => e.ItemId).HasColumnName("item_id");

<<<<<<< HEAD
                entity.Property(e => e.CountItemsSold).HasColumnName("Count_items_sold");

=======
>>>>>>> main
                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.ItemName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("item_name");

                entity.Property(e => e.ItemType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("item_type");

                entity.Property(e => e.MenuId).HasColumnName("menu_id");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.MenuItems)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_menu_items_menus");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.AddressId).HasColumnName("address_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasColumnName("order_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("payment_method");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("total_amount");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.AddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_orders_addresses");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_orders_users");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("order_items");

                entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");

                entity.Property(e => e.ItemId).HasColumnName("item_id");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_order_items_menu_items");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_order_items_orders");
            });

            modelBuilder.Entity<RecommendedMeal>(entity =>
            {
                entity.HasKey(e => e.RecommendId)
<<<<<<< HEAD
                    .HasName("PK__recommen__5D4DAA7192577175");
=======
                    .HasName("PK__recommen__5D4DAA716AB3C18C");
>>>>>>> main

                entity.ToTable("recommended_meals");

                entity.Property(e => e.RecommendId).HasColumnName("recommend_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ItemId).HasColumnName("item_id");

                entity.Property(e => e.Score)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("score");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.RecommendedMeals)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_recommended_meals_items");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RecommendedMeals)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_recommended_meals_users");
            });

            modelBuilder.Entity<RevenueReport>(entity =>
            {
                entity.HasKey(e => e.ReportId)
<<<<<<< HEAD
                    .HasName("PK__revenue___779B7C58B4418CC9");
=======
                    .HasName("PK__revenue___779B7C58397A8C80");
>>>>>>> main

                entity.ToTable("revenue_reports");

                entity.Property(e => e.ReportId).HasColumnName("report_id");

                entity.Property(e => e.GeneratedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("generated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ManagerId).HasColumnName("manager_id");

                entity.Property(e => e.ReportDate)
                    .HasColumnType("date")
                    .HasColumnName("report_date");

                entity.Property(e => e.TotalOrders).HasColumnName("total_orders");

                entity.Property(e => e.TotalRevenue)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("total_revenue");

                entity.HasOne(d => d.Manager)
                    .WithMany(p => p.RevenueReports)
                    .HasForeignKey(d => d.ManagerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_revenue_reports_users");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

<<<<<<< HEAD
                entity.HasIndex(e => e.Email, "UQ__users__AB6E6164B21CD19C")
=======
                entity.HasIndex(e => e.Email, "UQ__users__AB6E6164380A30B7")
>>>>>>> main
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DefaultCuisine)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("default_cuisine");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.FullName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("full_name");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.Property(e => e.Role)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("role");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
