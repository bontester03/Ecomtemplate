namespace KhanHomeFloralLine.Api.Models;

public sealed record UpdateOrderStatusRequest(string Status, string? Note);
public sealed record ApplyPromoRequest(string Code, decimal Subtotal);
public sealed record InitiatePaymentApiRequest(Guid OrderId, string ReturnUrl, string CancelUrl);
public sealed record PaymentWebhookApiRequest(string GatewayReference, string Status, decimal Amount);
public sealed record ProductVariantConfigRequest(Guid? Id, string Name, decimal Price, bool IsDefault, bool IsActive);
public sealed record ProductConfigUpdateRequest(IReadOnlyCollection<ProductVariantConfigRequest> Variants, IReadOnlyCollection<Guid> AddOnIds);
public sealed record DeliverySettingsUpdateRequest(int SameDayCutoffHour);
public sealed record AdminUserAccessUpdateRequest(bool IsActive);
public sealed record AdminUserListItemDto(
    string Id,
    string UserType,
    string FullName,
    string? Email,
    string? Phone,
    string Role,
    bool IsActive,
    bool CanToggleAccess,
    string Address,
    string Area,
    int OrdersCount,
    DateTime CreatedAtUtc,
    DateTime? LastOrderAtUtc);

