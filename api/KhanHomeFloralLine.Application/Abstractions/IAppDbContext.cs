using KhanHomeFloralLine.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KhanHomeFloralLine.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductImage> ProductImages { get; }
    DbSet<ProductVariant> ProductVariants { get; }
    DbSet<AddOn> AddOns { get; }
    DbSet<ProductAddOn> ProductAddOns { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<OrderItemAddOn> OrderItemAddOns { get; }
    DbSet<DeliveryZone> DeliveryZones { get; }
    DbSet<DeliveryTimeSlot> DeliveryTimeSlots { get; }
    DbSet<PromoCode> PromoCodes { get; }
    DbSet<PromoCodeUsage> PromoCodeUsages { get; }
    DbSet<Payment> Payments { get; }
    DbSet<OrderStatusHistory> OrderStatusHistories { get; }
    DbSet<AppSetting> AppSettings { get; }
    DbSet<CartItem> CartItems { get; }
    DbSet<CartItemAddOn> CartItemAddOns { get; }
    DbSet<UserAddress> UserAddresses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

