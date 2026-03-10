using KhanHomeFloralLine.Application.Abstractions;
using KhanHomeFloralLine.Application.Promotions;
using KhanHomeFloralLine.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace KhanHomeFloralLine.Infrastructure.Services;

public class PromoService(IAppDbContext dbContext) : IPromoService
{
    public async Task<PromoValidationResult> ValidateAsync(PromoValidationRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return new PromoValidationResult(false, 0, "Promo code is required");
        }

        var promo = await dbContext.PromoCodes.FirstOrDefaultAsync(
            x => x.Code == request.Code.ToUpper() && x.IsActive,
            cancellationToken);

        if (promo is null)
        {
            return new PromoValidationResult(false, 0, "Invalid promo code");
        }

        if (promo.ExpiresAtUtc < request.NowUtc)
        {
            return new PromoValidationResult(false, 0, "Promo code is expired");
        }

        if (request.Subtotal < promo.MinSpend)
        {
            return new PromoValidationResult(false, 0, $"Minimum spend is {promo.MinSpend:0.00} AED");
        }

        if (promo.UsageLimit.HasValue)
        {
            var count = await dbContext.PromoCodeUsages.CountAsync(x => x.PromoCodeId == promo.Id, cancellationToken);
            if (count >= promo.UsageLimit.Value)
            {
                return new PromoValidationResult(false, 0, "Promo code usage limit reached");
            }
        }

        var discount = promo.Type switch
        {
            PromoType.FixedAmount => promo.Value,
            PromoType.Percentage => request.Subtotal * promo.Value / 100m,
            _ => 0
        };

        discount = decimal.Round(Math.Min(discount, request.Subtotal), 2, MidpointRounding.AwayFromZero);

        return new PromoValidationResult(true, discount, null, promo.Id);
    }
}

