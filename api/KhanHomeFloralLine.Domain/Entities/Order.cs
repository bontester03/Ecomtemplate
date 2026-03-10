using KhanHomeFloralLine.Domain.Common;
using KhanHomeFloralLine.Domain.Enums;

namespace KhanHomeFloralLine.Domain.Entities;

public class Order : BaseEntity
{
    public Guid? UserId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public string DeliveryArea { get; set; } = string.Empty;
    public DateOnly DeliveryDate { get; set; }
    public Guid DeliveryTimeSlotId { get; set; }
    public Guid DeliveryZoneId { get; set; }
    public string? MessageCard { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal DeliveryCharge { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsPaid { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? PromoCode { get; set; }

    public User? User { get; set; }
    public DeliveryTimeSlot? DeliveryTimeSlot { get; set; }
    public DeliveryZone? DeliveryZone { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

