using KhanHomeFloralLine.Application.Abstractions;
using KhanHomeFloralLine.Application.Delivery;
using KhanHomeFloralLine.Application.Promotions;
using KhanHomeFloralLine.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhanHomeFloralLine.Api.Controllers;

[ApiController]
[Route("api/meta")]
public class MetaController(IAppDbContext dbContext, IDeliveryService deliveryService, IPromoService promoService) : ControllerBase
{
    [HttpGet("delivery-zones")]
    public async Task<IActionResult> DeliveryZones(CancellationToken cancellationToken)
    {
        return Ok(await dbContext.DeliveryZones.Where(x => x.IsActive).OrderBy(x => x.Emirate).ThenBy(x => x.Area).ToListAsync(cancellationToken));
    }

    [HttpGet("delivery-slots")]
    public async Task<IActionResult> DeliverySlots([FromQuery] DateOnly? deliveryDate, CancellationToken cancellationToken)
    {
        var slots = await dbContext.DeliveryTimeSlots.Where(x => x.IsActive).OrderBy(x => x.StartTime).ToListAsync(cancellationToken);

        if (!deliveryDate.HasValue)
        {
            return Ok(slots);
        }

        var bookedCounts = await dbContext.Orders
            .Where(x => x.DeliveryDate == deliveryDate.Value && x.Status != OrderStatus.Cancelled && x.Status != OrderStatus.Refunded)
            .GroupBy(x => x.DeliveryTimeSlotId)
            .Select(g => new { SlotId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.SlotId, x => x.Count, cancellationToken);

        var response = slots.Select(slot =>
        {
            bookedCounts.TryGetValue(slot.Id, out var booked);
            var isAvailable = !slot.CapacityLimit.HasValue || slot.CapacityLimit.Value <= 0 || booked < slot.CapacityLimit.Value;
            return new
            {
                slot.Id,
                slot.Label,
                slot.StartTime,
                slot.EndTime,
                slot.CapacityLimit,
                booked,
                isAvailable
            };
        });

        return Ok(response);
    }

    [HttpPost("validate-delivery")]
    public async Task<IActionResult> ValidateDelivery(DeliveryValidationRequest request, CancellationToken cancellationToken)
    {
        var result = await deliveryService.ValidateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("validate-promo")]
    public async Task<IActionResult> ValidatePromo(PromoValidationRequest request, CancellationToken cancellationToken)
    {
        var result = await promoService.ValidateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("sitemap")]
    public async Task<IActionResult> SitemapData(CancellationToken cancellationToken)
    {
        var products = await dbContext.Products.Where(x => x.IsActive).Select(x => new { type = "product", slug = x.Slug, updatedAt = x.UpdatedAtUtc ?? x.CreatedAtUtc }).ToListAsync(cancellationToken);
        var categories = await dbContext.Categories.Where(x => x.IsActive).Select(x => new { type = "category", slug = x.Slug, updatedAt = x.UpdatedAtUtc ?? x.CreatedAtUtc }).ToListAsync(cancellationToken);
        return Ok(products.Concat(categories));
    }
}
