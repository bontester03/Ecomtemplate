using KhanHomeFloralLine.Application.Promotions;

namespace KhanHomeFloralLine.Application.Abstractions;

public interface IPromoService
{
    Task<PromoValidationResult> ValidateAsync(PromoValidationRequest request, CancellationToken cancellationToken = default);
}

