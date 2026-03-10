using KhanHomeFloralLine.Domain.Enums;

namespace KhanHomeFloralLine.Application.Orders;

public sealed record CheckoutItemRequest(Guid ProductId, Guid ProductVariantId, int Quantity, IReadOnlyCollection<Guid> AddOnIds, string? MessageCard);

public sealed record CheckoutRequest(
    Guid? UserId,
    string CustomerName,
    string Email,
    string Phone,
    string DeliveryAddress,
    string DeliveryArea,
    Guid DeliveryZoneId,
    Guid DeliveryTimeSlotId,
    DateOnly DeliveryDate,
    string? PromoCode,
    PaymentMethod PaymentMethod,
    bool SaveAddress,
    string? AddressLabel,
    IReadOnlyCollection<CheckoutItemRequest> Items);

public sealed record CreateOrderResult(Guid OrderId, string OrderNumber, decimal Total, string Status);

public sealed record TrackingRequest(string OrderNumber, string Contact);

public sealed record OrderSummaryDto(Guid Id, string OrderNumber, DateTime CreatedAtUtc, decimal TotalAmount, string Status, bool IsPaid);

public sealed record OrderDetailDto(
    Guid Id,
    string OrderNumber,
    string CustomerName,
    string Email,
    string Phone,
    string DeliveryAddress,
    string DeliveryArea,
    DateOnly DeliveryDate,
    string DeliveryTimeSlot,
    decimal Subtotal,
    decimal DiscountAmount,
    decimal DeliveryCharge,
    decimal TotalAmount,
    string Status,
    bool IsPaid,
    IReadOnlyCollection<OrderDetailItemDto> Items,
    IReadOnlyCollection<OrderStatusTimelineDto> Timeline);

public sealed record OrderDetailItemDto(string ProductName, string VariantName, int Quantity, decimal UnitPrice, decimal AddOnTotal, string? MessageCard, IReadOnlyCollection<string> AddOns);

public sealed record OrderStatusTimelineDto(string Status, DateTime CreatedAtUtc, string? Note);

