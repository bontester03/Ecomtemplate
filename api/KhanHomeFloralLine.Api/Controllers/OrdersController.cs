using KhanHomeFloralLine.Api.Extensions;
using KhanHomeFloralLine.Application.Abstractions;
using KhanHomeFloralLine.Application.Common;
using KhanHomeFloralLine.Application.Orders;
using KhanHomeFloralLine.Domain.Entities;
using KhanHomeFloralLine.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhanHomeFloralLine.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(
    IAppDbContext dbContext,
    IPromoService promoService,
    IDeliveryService deliveryService,
    IOrderPricingService pricingService,
    IOrderNumberGenerator orderNumberGenerator) : ControllerBase
{
    [HttpPost("checkout")]
    [AllowAnonymous]
    public async Task<ActionResult<CreateOrderResult>> Checkout(CheckoutRequest request, CancellationToken cancellationToken)
    {
        var authenticatedUserId = User.GetUserId();
        var authenticatedEmail = User.GetEmail();
        var effectiveUserId = authenticatedUserId ?? request.UserId;
        var effectiveEmail = !string.IsNullOrWhiteSpace(authenticatedEmail) ? authenticatedEmail : request.Email;

        if (request.Items.Count == 0)
        {
            throw new AppValidationException("Cart is empty");
        }

        if (request.Items.Any(x => !string.IsNullOrEmpty(x.MessageCard) && x.MessageCard!.Length > 250))
        {
            throw new AppValidationException("Message card exceeds 250 characters");
        }

        var variantIds = request.Items.Select(x => x.ProductVariantId).Distinct().ToList();
        var variants = await dbContext.ProductVariants
            .Include(v => v.Product)
            .Where(v => variantIds.Contains(v.Id) && v.IsActive)
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        if (variants.Count != variantIds.Count)
        {
            throw new AppValidationException("One or more selected variants are invalid");
        }

        var addOnIds = request.Items.SelectMany(x => x.AddOnIds).Distinct().ToList();
        var addOns = await dbContext.AddOns.Where(x => addOnIds.Contains(x.Id) && x.IsActive)
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var linePricing = request.Items.Select(item =>
        {
            var variant = variants[item.ProductVariantId];
            var addOnTotal = item.AddOnIds.Sum(id => addOns.TryGetValue(id, out var addOn) ? addOn.Price : 0m);
            return (variant.Price, item.Quantity, addOnTotal);
        }).ToList();

        var subtotal = pricingService.CalculateSubtotal(linePricing);

        var deliveryResult = await deliveryService.ValidateAsync(new(
            request.DeliveryZoneId,
            request.DeliveryTimeSlotId,
            request.DeliveryDate,
            DateTime.UtcNow), cancellationToken);

        if (!deliveryResult.IsValid)
        {
            throw new AppValidationException(deliveryResult.Error ?? "Invalid delivery request");
        }

        decimal discount = 0;
        Guid? promoCodeId = null;
        if (!string.IsNullOrWhiteSpace(request.PromoCode))
        {
            var promoResult = await promoService.ValidateAsync(new(request.PromoCode, subtotal, DateTime.UtcNow), cancellationToken);
            if (!promoResult.IsValid)
            {
                throw new AppValidationException(promoResult.Error ?? "Invalid promo code");
            }

            discount = promoResult.DiscountAmount;
            promoCodeId = promoResult.PromoCodeId;
        }

        var total = pricingService.CalculateTotal(subtotal, discount, deliveryResult.DeliveryCharge);

        var selectedZone = await dbContext.DeliveryZones
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.DeliveryZoneId && x.IsActive, cancellationToken)
            ?? throw new AppValidationException("Invalid delivery zone");

        if (!AreaMatchesZone(request.DeliveryArea, selectedZone.Area))
        {
            throw new AppValidationException($"Delivery area must match the selected delivery zone area ({selectedZone.Area})");
        }

        var order = new Order
        {
            UserId = effectiveUserId,
            OrderNumber = orderNumberGenerator.Generate(),
            CustomerName = request.CustomerName,
            Email = effectiveEmail,
            Phone = request.Phone,
            DeliveryAddress = request.DeliveryAddress,
            DeliveryArea = request.DeliveryArea,
            DeliveryDate = request.DeliveryDate,
            DeliveryTimeSlotId = request.DeliveryTimeSlotId,
            DeliveryZoneId = request.DeliveryZoneId,
            MessageCard = string.Join(" | ", request.Items.Select(x => x.MessageCard).Where(x => !string.IsNullOrWhiteSpace(x))),
            Subtotal = subtotal,
            DiscountAmount = discount,
            DeliveryCharge = deliveryResult.DeliveryCharge,
            TotalAmount = total,
            PaymentMethod = request.PaymentMethod,
            PromoCode = request.PromoCode?.ToUpperInvariant(),
            Status = OrderStatus.Pending,
            IsPaid = false
        };

        dbContext.Orders.Add(order);

        foreach (var item in request.Items)
        {
            var variant = variants[item.ProductVariantId];
            var orderItem = new OrderItem
            {
                Order = order,
                ProductId = item.ProductId,
                ProductVariantId = item.ProductVariantId,
                ProductNameSnapshot = variant.Product?.Name ?? "Product",
                VariantNameSnapshot = variant.Name,
                UnitPrice = variant.Price,
                Quantity = item.Quantity
            };
            dbContext.OrderItems.Add(orderItem);

            foreach (var addOnId in item.AddOnIds)
            {
                if (!addOns.TryGetValue(addOnId, out var addOn))
                {
                    continue;
                }

                dbContext.OrderItemAddOns.Add(new OrderItemAddOn
                {
                    OrderItem = orderItem,
                    AddOnId = addOnId,
                    AddOnNameSnapshot = addOn.Name,
                    PriceSnapshot = addOn.Price
                });
            }
        }

        dbContext.OrderStatusHistories.Add(new OrderStatusHistory
        {
            Order = order,
            Status = OrderStatus.Pending,
            Note = "Order created"
        });

        if (promoCodeId.HasValue)
        {
            dbContext.PromoCodeUsages.Add(new PromoCodeUsage
            {
                PromoCodeId = promoCodeId.Value,
                UserId = effectiveUserId,
                Order = order,
                UsedAtUtc = DateTime.UtcNow
            });
        }

        if (effectiveUserId.HasValue && request.SaveAddress)
        {
            dbContext.UserAddresses.Add(new UserAddress
            {
                Id = Guid.NewGuid(),
                UserId = effectiveUserId.Value,
                Label = string.IsNullOrWhiteSpace(request.AddressLabel) ? "Checkout Address" : request.AddressLabel.Trim(),
                RecipientName = string.IsNullOrWhiteSpace(request.CustomerName) ? "Recipient" : request.CustomerName.Trim(),
                Phone = request.Phone.Trim(),
                AddressLine = request.DeliveryAddress.Trim(),
                Area = request.DeliveryArea.Trim(),
                Emirate = selectedZone.Emirate.Trim(),
                Landmark = null,
                IsDefault = false,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new CreateOrderResult(order.Id, order.OrderNumber, order.TotalAmount, order.Status.ToString()));
    }

    [HttpGet("track")]
    [AllowAnonymous]
    public async Task<ActionResult<OrderDetailDto>> Track([FromQuery] string orderNumber, [FromQuery] string? contact, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
        {
            throw new AppValidationException("Order number is required");
        }

        var trimmedOrderNumber = orderNumber.Trim();
        var trimmedContact = contact?.Trim();
        var userId = User.GetUserId();
        var userEmail = User.GetEmail();

        var query = dbContext.Orders
            .Include(x => x.OrderItems).ThenInclude(x => x.AddOns)
            .Include(x => x.DeliveryTimeSlot)
            .Include(x => x.StatusHistory)
            .Where(x => x.OrderNumber == trimmedOrderNumber);

        Order? order;
        if (userId.HasValue)
        {
            order = await query.FirstOrDefaultAsync(
                x => x.UserId == userId.Value
                     || (!string.IsNullOrWhiteSpace(userEmail) && x.Email == userEmail),
                cancellationToken);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(trimmedContact))
            {
                throw new AppValidationException("Email or phone is required for guest tracking");
            }

            order = await query.FirstOrDefaultAsync(
                x => x.Email == trimmedContact || x.Phone == trimmedContact,
                cancellationToken);
        }

        if (order is null)
        {
            return NotFound();
        }

        return Ok(MapOrderDetail(order));
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyCollection<OrderSummaryDto>>> MyOrders(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId() ?? throw new AppValidationException("Missing user id");
        var email = User.GetEmail();

        var orders = await dbContext.Orders
            .Where(x => x.UserId == userId || (x.UserId == null && !string.IsNullOrWhiteSpace(email) && x.Email == email))
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new OrderSummaryDto(x.Id, x.OrderNumber, x.CreatedAtUtc, x.TotalAmount, x.Status.ToString(), x.IsPaid))
            .ToListAsync(cancellationToken);

        return Ok(orders);
    }

    [HttpGet("my/{orderId:guid}")]
    [Authorize]
    public async Task<ActionResult<OrderDetailDto>> MyOrderDetail(Guid orderId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId() ?? throw new AppValidationException("Missing user id");
        var email = User.GetEmail();

        var order = await dbContext.Orders
            .Include(x => x.OrderItems).ThenInclude(x => x.AddOns)
            .Include(x => x.DeliveryTimeSlot)
            .Include(x => x.StatusHistory)
            .FirstOrDefaultAsync(
                x => x.Id == orderId
                     && (x.UserId == userId || (x.UserId == null && !string.IsNullOrWhiteSpace(email) && x.Email == email)),
                cancellationToken);

        return order is null ? NotFound() : Ok(MapOrderDetail(order));
    }

    [HttpGet("{orderId:guid}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<ActionResult<OrderDetailDto>> GetOrder(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .Include(x => x.OrderItems).ThenInclude(x => x.AddOns)
            .Include(x => x.DeliveryTimeSlot)
            .Include(x => x.StatusHistory)
            .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);

        return order is null ? NotFound() : Ok(MapOrderDetail(order));
    }

    [HttpPatch("{orderId:guid}/status")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> UpdateStatus(Guid orderId, [FromBody] string status, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken)
                   ?? throw new AppNotFoundException("Order not found");

        if (!Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
        {
            throw new AppValidationException("Invalid status");
        }

        order.Status = parsedStatus;
        order.UpdatedAtUtc = DateTime.UtcNow;

        dbContext.OrderStatusHistories.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = parsedStatus,
            Note = $"Updated by {User.Identity?.Name ?? "admin"}"
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static OrderDetailDto MapOrderDetail(Order order)
    {
        return new OrderDetailDto(
            order.Id,
            order.OrderNumber,
            order.CustomerName,
            order.Email,
            order.Phone,
            order.DeliveryAddress,
            order.DeliveryArea,
            order.DeliveryDate,
            order.DeliveryTimeSlot?.Label ?? string.Empty,
            order.Subtotal,
            order.DiscountAmount,
            order.DeliveryCharge,
            order.TotalAmount,
            order.Status.ToString(),
            order.IsPaid,
            order.OrderItems.Select(i => new OrderDetailItemDto(
                i.ProductNameSnapshot,
                i.VariantNameSnapshot,
                i.Quantity,
                i.UnitPrice,
                i.AddOns.Sum(x => x.PriceSnapshot),
                null,
                i.AddOns.Select(a => a.AddOnNameSnapshot).ToList())).ToList(),
            order.StatusHistory.OrderBy(x => x.CreatedAtUtc)
                .Select(x => new OrderStatusTimelineDto(x.Status.ToString(), x.CreatedAtUtc, x.Note)).ToList());
    }

    private static bool AreaMatchesZone(string area, string zoneArea)
    {
        return Normalize(area) == Normalize(zoneArea);
    }

    private static string Normalize(string value)
    {
        return string.Concat(value.Where(char.IsLetterOrDigit)).ToUpperInvariant();
    }
}

