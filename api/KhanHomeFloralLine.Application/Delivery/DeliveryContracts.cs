namespace KhanHomeFloralLine.Application.Delivery;

public sealed record DeliveryValidationRequest(Guid DeliveryZoneId, Guid DeliveryTimeSlotId, DateOnly DeliveryDate, DateTime NowUtc);
public sealed record DeliveryValidationResult(bool IsValid, decimal DeliveryCharge, string? Error);

