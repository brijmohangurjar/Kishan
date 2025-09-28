using Microsoft.EntityFrameworkCore;
using KrishiClinic.API.Models;

namespace KrishiClinic.API.Data
{
    public class KrishiClinicDbContext : DbContext
    {
        public KrishiClinicDbContext(DbContextOptions<KrishiClinicDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SaleBuyCategory> SaleBuyCategories { get; set; }
        public DbSet<SaleBuyProduct> SaleBuyProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Village).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Mobile).IsRequired().HasMaxLength(10);
                entity.Property(e => e.OTP).HasMaxLength(6);
                entity.HasIndex(e => e.Mobile).IsUnique();
            });

            // Product entity configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.HasOne(e => e.CategoryNavigation)
                      .WithMany(c => c.Products)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Cart entity configuration
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.CartId);
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Carts)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Product)
                      .WithMany(p => p.Carts)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.UserId, e.ProductId }).IsUnique();
            });

            // Order entity configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(20);
                entity.Property(e => e.DeliveryAddress).HasMaxLength(500);
                entity.Property(e => e.DeliveryVillage).HasMaxLength(100);
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => e.OrderNumber).IsUnique();
            });

            // OrderItem entity configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.OrderItemId);
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(10,2)");
                entity.Property(e => e.ProductImageUrl).HasMaxLength(500);
                entity.HasOne(e => e.Order)
                      .WithMany(o => o.OrderItems)
                      .HasForeignKey(e => e.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Product)
                      .WithMany(p => p.OrderItems)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Admin entity configuration
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.AdminId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).HasMaxLength(20);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Video entity configuration
            modelBuilder.Entity<Video>(entity =>
            {
                entity.HasKey(e => e.VideoId);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.VideoUrl).IsRequired().HasMaxLength(500);
                entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.HasOne(e => e.CreatedByAdmin)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedBy)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Category entity configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // SaleBuyCategory entity configuration
            modelBuilder.Entity<SaleBuyCategory>(entity =>
            {
                entity.HasKey(e => e.SaleBuyCategoryId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // SaleBuyProduct entity configuration
            modelBuilder.Entity<SaleBuyProduct>(entity =>
            {
                entity.HasKey(e => e.SaleBuyProductId);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PlaceName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ProductDescription).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(15);
                entity.Property(e => e.ImageUrls).HasMaxLength(2000);
                entity.HasOne(e => e.SaleBuyCategory)
                      .WithMany(c => c.SaleBuyProducts)
                      .HasForeignKey(e => e.SaleBuyCategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed data for Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    Name = "Organic Seeds - Tomato",
                    Description = "High-quality organic tomato seeds for healthy growth. Perfect for home gardening.",
                    Price = 299,
                    ImageUrl = "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=300&h=300&fit=crop",
                    Category = "Seeds",
                    StockQuantity = 100,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 2,
                    Name = "Fertilizer - NPK 19:19:19",
                    Description = "Balanced NPK fertilizer for all types of crops. Promotes healthy plant growth.",
                    Price = 450,
                    ImageUrl = "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop",
                    Category = "Fertilizers",
                    StockQuantity = 50,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 3,
                    Name = "Bayer Crop Science - Confidor",
                    Description = "Systemic insecticide for controlling sucking pests like aphids, whiteflies, and thrips.",
                    Price = 450,
                    ImageUrl = "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop",
                    Category = "Pesticides",
                    StockQuantity = 75,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 4,
                    Name = "Syngenta - Tilt",
                    Description = "Fungicide for controlling leaf spot, rust and powdery mildew in various crops.",
                    Price = 650,
                    ImageUrl = "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop",
                    Category = "Fungicides",
                    StockQuantity = 60,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 5,
                    Name = "DAP Fertilizer - 18:46:0",
                    Description = "Diammonium phosphate fertilizer for promoting root development and flowering.",
                    Price = 320,
                    ImageUrl = "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop",
                    Category = "Fertilizers",
                    StockQuantity = 80,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 6,
                    Name = "Urea - 46:0:0",
                    Description = "High nitrogen content urea fertilizer for vegetative growth and green foliage.",
                    Price = 280,
                    ImageUrl = "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop",
                    Category = "Fertilizers",
                    StockQuantity = 120,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 7,
                    Name = "Roundup - Glyphosate",
                    Description = "Non-selective herbicide for controlling weeds in agricultural fields.",
                    Price = 580,
                    ImageUrl = "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop",
                    Category = "Herbicides",
                    StockQuantity = 45,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 8,
                    Name = "Organic Seeds - Chilli",
                    Description = "Premium quality chilli seeds with high yield potential and disease resistance.",
                    Price = 350,
                    ImageUrl = "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=300&h=300&fit=crop",
                    Category = "Seeds",
                    StockQuantity = 90,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 9,
                    Name = "MOP Fertilizer - 0:0:60",
                    Description = "Muriate of Potash for improving fruit quality and disease resistance.",
                    Price = 380,
                    ImageUrl = "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop",
                    Category = "Fertilizers",
                    StockQuantity = 70,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 10,
                    Name = "Imidacloprid - 17.8% SL",
                    Description = "Systemic insecticide for controlling aphids, jassids, and whiteflies in cotton and vegetables.",
                    Price = 520,
                    ImageUrl = "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop",
                    Category = "Insecticides",
                    StockQuantity = 55,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 11,
                    Name = "Organic Seeds - Brinjal",
                    Description = "High-yielding brinjal seeds with excellent fruit quality and market demand.",
                    Price = 420,
                    ImageUrl = "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=300&h=300&fit=crop",
                    Category = "Seeds",
                    StockQuantity = 85,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 12,
                    Name = "Mancozeb - 75% WP",
                    Description = "Contact fungicide for controlling early and late blight in potato and tomato crops.",
                    Price = 480,
                    ImageUrl = "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop",
                    Category = "Fungicides",
                    StockQuantity = 65,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 13,
                    Name = "SSP Fertilizer - 0:16:0",
                    Description = "Single Super Phosphate for root development and early crop establishment.",
                    Price = 340,
                    ImageUrl = "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop",
                    Category = "Fertilizers",
                    StockQuantity = 95,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 14,
                    Name = "2,4-D Herbicide",
                    Description = "Selective herbicide for controlling broadleaf weeds in wheat and rice fields.",
                    Price = 620,
                    ImageUrl = "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop",
                    Category = "Herbicides",
                    StockQuantity = 40,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 15,
                    Name = "Organic Seeds - Onion",
                    Description = "Premium onion seeds with high bulb weight and good storage quality.",
                    Price = 380,
                    ImageUrl = "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=300&h=300&fit=crop",
                    Category = "Seeds",
                    StockQuantity = 75,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 16,
                    Name = "Chlorpyrifos - 20% EC",
                    Description = "Organophosphate insecticide for controlling soil and foliar pests in various crops.",
                    Price = 580,
                    ImageUrl = "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop",
                    Category = "Insecticides",
                    StockQuantity = 50,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 17,
                    Name = "Complex Fertilizer - 12:32:16",
                    Description = "Balanced complex fertilizer for flowering and fruiting crops like tomato and brinjal.",
                    Price = 420,
                    ImageUrl = "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop",
                    Category = "Fertilizers",
                    StockQuantity = 60,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 18,
                    Name = "Carbendazim - 50% WP",
                    Description = "Systemic fungicide for controlling sheath blight, blast and other fungal diseases.",
                    Price = 550,
                    ImageUrl = "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop",
                    Category = "Fungicides",
                    StockQuantity = 45,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 19,
                    Name = "Organic Seeds - Okra",
                    Description = "High-yielding okra seeds with tender, long pods suitable for market and home use.",
                    Price = 320,
                    ImageUrl = "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=300&h=300&fit=crop",
                    Category = "Seeds",
                    StockQuantity = 80,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 20,
                    Name = "Paraquat - 24% SL",
                    Description = "Contact herbicide for controlling weeds in plantation crops and non-crop areas.",
                    Price = 680,
                    ImageUrl = "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop",
                    Category = "Herbicides",
                    StockQuantity = 35,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 21,
                    Name = "Organic Seeds - Cucumber",
                    Description = "Premium cucumber seeds with high yield, disease resistance and excellent fruit quality.",
                    Price = 290,
                    ImageUrl = "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=300&h=300&fit=crop",
                    Category = "Seeds",
                    StockQuantity = 70,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 22,
                    Name = "Lambda Cyhalothrin - 5% EC",
                    Description = "Synthetic pyrethroid insecticide for controlling lepidopteran pests in cotton and vegetables.",
                    Price = 720,
                    ImageUrl = "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop",
                    Category = "Insecticides",
                    StockQuantity = 40,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    ProductId = 23,
                    Name = "Micronutrient Mixture",
                    Description = "Essential micronutrients including zinc, boron, iron, and manganese for healthy crop growth.",
                    Price = 890,
                    ImageUrl = "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop",
                    Category = "Micronutrients",
                    StockQuantity = 25,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed data for Admin
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    AdminId = 1,
                    Name = "Brijmohan Gurjar",
                    Email = "brijmohangurjar48@gmail.com",
                    Password = "$2a$11$rQZ8K3tXvL7hM9nP2qR1te8K5wF3sA6bC9dE1fG4hI7jK0lM3nP6q", // This is 'Admin@123' hashed with BCrypt
                    Role = "SuperAdmin",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
