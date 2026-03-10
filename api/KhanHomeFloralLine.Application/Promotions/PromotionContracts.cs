namespace KhanHomeFloralLine.Application.Promotions;

public sealed record PromoValidationRequest(string Code, decimal Subtotal, DateTime NowUtc);
public sealed record PromoValidationResult(bool IsValid, decimal DiscountAmount, string? Error, Guid? PromoCodeId = null);

