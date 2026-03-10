using KhanHomeFloralLine.Application.Abstractions;
using KhanHomeFloralLine.Domain.Entities;
using KhanHomeFloralLine.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KhanHomeFloralLine.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options), IAppDbContext
{
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<AddOn> AddOns => Set<AddOn>();
    public DbSet<ProductAddOn> ProductAddOns => Set<ProductAddOn>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderItemAddOn> OrderItemAddOns => Set<OrderItemAddOn>();
    public DbSet<DeliveryZone> DeliveryZones => Set<DeliveryZone>();
    public DbSet<DeliveryTimeSlot> DeliveryTimeSlots => Set<DeliveryTimeSlot>();
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
    public DbSet<PromoCodeUsage> PromoCodeUsages => Set<PromoCodeUsage>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<CartItemAddOn> CartItemAddOns => Set<CartItemAddOn>();
    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<User>(e =>
        {
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(120);
            e.Property(x => x.FullName).HasMaxLength(120);
            e.Property(x => x.PhoneNumber).HasMaxLength(30);
        });

        builder.Entity<UserAddress>(e =>
        {
            e.Property(x => x.Label).HasMaxLength(40);
            e.Property(x => x.RecipientName).HasMaxLength(120);
            e.Property(x => x.Phone).HasMaxLength(30);
            e.Property(x => x.AddressLine).HasMaxLength(240);
            e.Property(x => x.Area).HasMaxLength(120);
            e.Property(x => x.Emirate).HasMaxLength(80);
            e.Property(x => x.Landmark).HasMaxLength(180);
            e.HasOne(x => x.User).WithMany(x => x.Addresses).HasForeignKey(x => x.UserId);
        });

        builder.Entity<RefreshToken>(e =>
        {
            e.HasIndex(x => x.Token).IsUnique();
            e.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId);
        });

        builder.Entity<Category>(e =>
        {
            e.HasIndex(x => x.Slug).IsUnique();
            e.Property(x => x.Name).HasMaxLength(80);
            e.Property(x => x.Slug).HasMaxLength(80);
        });

        builder.Entity<Product>(e =>
        {
            e.HasIndex(x => x.Slug).IsUnique();
            e.Property(x => x.Name).HasMaxLength(120);
            e.HasOne(x => x.Category).WithMany(x => x.Products).HasForeignKey(x => x.CategoryId);
        });

        builder.Entity<ProductImage>(e =>
        {
            e.HasOne(x => x.Product).WithMany(x => x.Images).HasForeignKey(x => x.ProductId);
        });

        builder.Entity<ProductVariant>(e =>
        {
            e.Property(x => x.Price).HasPrecision(18, 2);
            e.HasOne(x => x.Product).WithMany(x => x.Variants).HasForeignKey(x => x.ProductId);
        });

        builder.Entity<AddOn>(e =>
        {
            e.Property(x => x.Price).HasPrecision(18, 2);
            e.Property(x => x.Name).HasMaxLength(80);
        });

        builder.Entity<ProductAddOn>(e =>
        {
            e.HasKey(x => new { x.ProductId, x.AddOnId });
            e.HasOne(x => x.Product).WithMany(x => x.ProductAddOns).HasForeignKey(x => x.ProductId);
            e.HasOne(x => x.AddOn).WithMany(x => x.ProductAddOns).HasForeignKey(x => x.AddOnId);
        });

        builder.Entity<Order>(e =>
        {
            e.HasIndex(x => x.OrderNumber).IsUnique();
            e.Property(x => x.Subtotal).HasPrecision(18, 2);
            e.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            e.Property(x => x.DeliveryCharge).HasPrecision(18, 2);
            e.Property(x => x.TotalAmount).HasPrecision(18, 2);
            e.HasOne(x => x.User).WithMany(x => x.Orders).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.DeliveryZone).WithMany().HasForeignKey(x => x.DeliveryZoneId);
            e.HasOne(x => x.DeliveryTimeSlot).WithMany().HasForeignKey(x => x.DeliveryTimeSlotId);
        });

        builder.Entity<OrderItem>(e =>
        {
            e.Property(x => x.UnitPrice).HasPrecision(18, 2);
            e.HasOne(x => x.Order).WithMany(x => x.OrderItems).HasForeignKey(x => x.OrderId);
        });

        builder.Entity<OrderItemAddOn>(e =>
        {
            e.Property(x => x.PriceSnapshot).HasPrecision(18, 2);
            e.HasOne(x => x.OrderItem).WithMany(x => x.AddOns).HasForeignKey(x => x.OrderItemId);
        });

        builder.Entity<DeliveryZone>(e =>
        {
            e.Property(x => x.Charge).HasPrecision(18, 2);
            e.HasIndex(x => new { x.Emirate, x.Area }).IsUnique();
        });

        builder.Entity<DeliveryTimeSlot>(e =>
        {
            e.Property(x => x.CapacityLimit).IsRequired(false);
        });

        builder.Entity<PromoCode>(e =>
        {
            e.Property(x => x.Value).HasPrecision(18, 2);
            e.Property(x => x.MinSpend).HasPrecision(18, 2);
            e.HasIndex(x => x.Code).IsUnique();
        });

        builder.Entity<Payment>(e =>
        {
            e.Property(x => x.Amount).HasPrecision(18, 2);
            e.HasOne(x => x.Order).WithMany(x => x.Payments).HasForeignKey(x => x.OrderId);
        });

        builder.Entity<OrderStatusHistory>(e =>
        {
            e.HasOne(x => x.Order).WithMany(x => x.StatusHistory).HasForeignKey(x => x.OrderId);
        });

        builder.Entity<AppSetting>(e =>
        {
            e.HasIndex(x => x.Key).IsUnique();
            e.Property(x => x.Key).HasMaxLength(80);
        });

        builder.Entity<CartItem>(e =>
        {
            e.HasOne(x => x.User).WithMany(x => x.CartItems).HasForeignKey(x => x.UserId);
            e.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
            e.HasOne(x => x.ProductVariant).WithMany().HasForeignKey(x => x.ProductVariantId);
        });

        builder.Entity<CartItemAddOn>(e =>
        {
            e.HasOne(x => x.CartItem).WithMany(x => x.AddOns).HasForeignKey(x => x.CartItemId);
            e.HasOne(x => x.AddOn).WithMany().HasForeignKey(x => x.AddOnId);
        });

        SeedData(builder);
    }

    private static void SeedData(ModelBuilder builder)
    {
        var catOccasion = Guid.Parse("22222222-2222-2222-2222-222222222221");
        var catBouquet = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var catAddon = Guid.Parse("22222222-2222-2222-2222-222222222223");

        var product1 = Guid.Parse("33333333-3333-3333-3333-333333333331");
        var product2 = Guid.Parse("33333333-3333-3333-3333-333333333332");
        var product3 = Guid.Parse("33333333-3333-3333-3333-333333333333");

        var addon1 = Guid.Parse("44444444-4444-4444-4444-444444444441");
        var addon2 = Guid.Parse("44444444-4444-4444-4444-444444444442");
        var addon3 = Guid.Parse("44444444-4444-4444-4444-444444444443");
        var addon4 = Guid.Parse("44444444-4444-4444-4444-444444444444");

        var variant1s = Guid.Parse("55555555-5555-5555-5555-555555555551");
        var variant1m = Guid.Parse("55555555-5555-5555-5555-555555555552");
        var variant1l = Guid.Parse("55555555-5555-5555-5555-555555555553");
        var variant2m = Guid.Parse("55555555-5555-5555-5555-555555555554");
        var variant3s = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var variant3m = Guid.Parse("55555555-5555-5555-5555-555555555556");
        var variant3l = Guid.Parse("55555555-5555-5555-5555-555555555557");

        var zoneDubai = Guid.Parse("66666666-6666-6666-6666-666666666661");
        var zoneAbuDhabi = Guid.Parse("66666666-6666-6666-6666-666666666662");

        var slot1 = Guid.Parse("77777777-7777-7777-7777-777777777771");
        var slot2 = Guid.Parse("77777777-7777-7777-7777-777777777772");
        var slot3 = Guid.Parse("77777777-7777-7777-7777-777777777773");

        var promo = Guid.Parse("88888888-8888-8888-8888-888888888881");
        var settingCutoff = Guid.Parse("99999999-9999-9999-9999-999999999991");

        builder.Entity<Category>().HasData(
            new Category { Id = catOccasion, Name = "Occasions", Slug = "occasions", IsFeatured = true, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new Category { Id = catBouquet, Name = "Bouquets", Slug = "bouquets", IsFeatured = true, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new Category { Id = catAddon, Name = "Add-ons", Slug = "addons", IsFeatured = false, IsActive = true, CreatedAtUtc = DateTime.UtcNow }
        );

        builder.Entity<Product>().HasData(
            new Product { Id = product1, CategoryId = catBouquet, Name = "Royal Red Roses", Slug = "royal-red-roses", Description = "Premium red rose arrangement", IsFeatured = true, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new Product { Id = product2, CategoryId = catOccasion, Name = "Pastel Love Mix", Slug = "pastel-love-mix", Description = "Soft pastel bouquet for special occasions", IsFeatured = true, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new Product { Id = product3, CategoryId = catBouquet, Name = "Grand White Elegance", Slug = "grand-white-elegance", Description = "Elegant white lily and rose arrangement.", IsFeatured = true, IsActive = true, CreatedAtUtc = DateTime.UtcNow }
        );

        builder.Entity<ProductImage>().HasData(
            new ProductImage { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), ProductId = product1, ImageUrl = "https://via.placeholder.com/800x600?text=Royal+Red+Roses", SortOrder = 1, CreatedAtUtc = DateTime.UtcNow },
            new ProductImage { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), ProductId = product2, ImageUrl = "https://via.placeholder.com/800x600?text=Pastel+Love+Mix", SortOrder = 1, CreatedAtUtc = DateTime.UtcNow },
            new ProductImage { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), ProductId = product3, ImageUrl = "https://via.placeholder.com/800x600?text=Grand+White+Elegance", SortOrder = 1, CreatedAtUtc = DateTime.UtcNow }
        );

        builder.Entity<AddOn>().HasData(
            new AddOn { Id = addon1, Name = "Belgian Chocolate Box", Price = 45m, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new AddOn { Id = addon2, Name = "Celebration Balloons", Price = 30m, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new AddOn { Id = addon3, Name = "Greeting Teddy Bear", Price = 55m, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new AddOn { Id = addon4, Name = "Premium Scented Candle", Price = 65m, IsActive = true, CreatedAtUtc = DateTime.UtcNow }
        );

        builder.Entity<ProductVariant>().HasData(
            new ProductVariant { Id = variant1s, ProductId = product1, Name = "Small", Price = 199m, IsDefault = true, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new ProductVariant { Id = variant1m, ProductId = product1, Name = "Medium", Price = 279m, IsDefault = false, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new ProductVariant { Id = variant1l, ProductId = product1, Name = "Large", Price = 359m, IsDefault = false, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new ProductVariant { Id = variant2m, ProductId = product2, Name = "Medium", Price = 249m, IsDefault = true, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new ProductVariant { Id = variant3s, ProductId = product3, Name = "Small", Price = 289m, IsDefault = true, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new ProductVariant { Id = variant3m, ProductId = product3, Name = "Medium", Price = 349m, IsDefault = false, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new ProductVariant { Id = variant3l, ProductId = product3, Name = "Large", Price = 419m, IsDefault = false, IsActive = true, CreatedAtUtc = DateTime.UtcNow }
        );

        builder.Entity<ProductAddOn>().HasData(
            new ProductAddOn { ProductId = product1, AddOnId = addon1 },
            new ProductAddOn { ProductId = product1, AddOnId = addon2 },
            new ProductAddOn { ProductId = product2, AddOnId = addon1 },
            new ProductAddOn { ProductId = product2, AddOnId = addon2 },
            new ProductAddOn { ProductId = product3, AddOnId = addon3 },
            new ProductAddOn { ProductId = product3, AddOnId = addon4 }
        );

        builder.Entity<DeliveryZone>().HasData(
            new DeliveryZone { Id = zoneDubai, Emirate = "Dubai", Area = "Dubai", Charge = 25m, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new DeliveryZone { Id = zoneAbuDhabi, Emirate = "Abu Dhabi", Area = "Abu Dhabi", Charge = 35m, IsActive = true, CreatedAtUtc = DateTime.UtcNow }
        );

        builder.Entity<DeliveryTimeSlot>().HasData(
            new DeliveryTimeSlot { Id = slot1, Label = "10:00-12:00", StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(12, 0), CapacityLimit = 50, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new DeliveryTimeSlot { Id = slot2, Label = "12:00-14:00", StartTime = new TimeOnly(12, 0), EndTime = new TimeOnly(14, 0), CapacityLimit = 50, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new DeliveryTimeSlot { Id = slot3, Label = "14:00-16:00", StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(16, 0), CapacityLimit = 50, IsActive = true, CreatedAtUtc = DateTime.UtcNow }
        );

        builder.Entity<PromoCode>().HasData(
            new PromoCode { Id = promo, Code = "WELCOME10", Type = PromoType.Percentage, Value = 10m, MinSpend = 150m, ExpiresAtUtc = new DateTime(2027, 12, 31, 0, 0, 0, DateTimeKind.Utc), IsActive = true, CreatedAtUtc = DateTime.UtcNow }
        );

        builder.Entity<AppSetting>().HasData(
            new AppSetting { Id = settingCutoff, Key = "SameDayCutoffHour", Value = "14", CreatedAtUtc = DateTime.UtcNow }
        );
    }
}




