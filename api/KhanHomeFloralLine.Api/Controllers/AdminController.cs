using KhanHomeFloralLine.Api.Extensions;
using KhanHomeFloralLine.Api.Models;
using KhanHomeFloralLine.Application.Abstractions;
using KhanHomeFloralLine.Application.Common;
using KhanHomeFloralLine.Domain.Entities;
using KhanHomeFloralLine.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhanHomeFloralLine.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin,Staff")]
public class AdminController(IAppDbContext dbContext, IBlobStorageService blobStorageService) : ControllerBase
{
    [HttpGet("orders")]
    public async Task<IActionResult> Orders([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string? status, CancellationToken cancellationToken)
    {
        var query = dbContext.Orders.AsQueryable();

        if (from.HasValue)
        {
            query = query.Where(x => x.CreatedAtUtc >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.CreatedAtUtc <= to.Value);
        }

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(x => x.Status == parsedStatus);
        }

        var results = await query.OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new
            {
                x.Id,
                x.OrderNumber,
                x.CustomerName,
                x.Phone,
                x.TotalAmount,
                Status = x.Status.ToString(),
                x.IsPaid,
                x.CreatedAtUtc
            }).ToListAsync(cancellationToken);

        return Ok(results);
    }

    [HttpGet("users")]
    public async Task<IActionResult> Users([FromQuery] string? q, CancellationToken cancellationToken)
    {
        var users = await dbContext.Users
            .AsNoTracking()
            .Select(x => new
            {
                x.Id,
                x.FullName,
                x.Email,
                Phone = x.PhoneNumber,
                Role = x.Role.ToString(),
                x.IsActive,
                x.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        var userOrderStats = await dbContext.Orders
            .AsNoTracking()
            .Where(x => x.UserId.HasValue)
            .GroupBy(x => x.UserId!.Value)
            .Select(g => new
            {
                UserId = g.Key,
                OrdersCount = g.Count(),
                LastOrderAtUtc = g.Max(x => x.CreatedAtUtc),
                LastAddress = g.OrderByDescending(x => x.CreatedAtUtc).Select(x => x.DeliveryAddress).FirstOrDefault(),
                LastArea = g.OrderByDescending(x => x.CreatedAtUtc).Select(x => x.DeliveryArea).FirstOrDefault()
            })
            .ToDictionaryAsync(x => x.UserId, cancellationToken);

        var guestOrderRows = await dbContext.Orders
            .AsNoTracking()
            .Where(x => x.UserId == null)
            .Select(x => new
            {
                x.CustomerName,
                x.Email,
                x.Phone,
                x.DeliveryAddress,
                x.DeliveryArea,
                x.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        var guestRows = guestOrderRows
            .GroupBy(x => new { x.Email, x.Phone })
            .Select(g =>
            {
                var latest = g.OrderByDescending(x => x.CreatedAtUtc).First();
                return new AdminUserListItemDto(
                    Id: $"guest:{latest.Email}:{latest.Phone}",
                    UserType: "Guest",
                    FullName: latest.CustomerName,
                    Email: latest.Email,
                    Phone: latest.Phone,
                    Role: "Guest",
                    IsActive: false,
                    CanToggleAccess: false,
                    Address: latest.DeliveryAddress,
                    Area: latest.DeliveryArea,
                    OrdersCount: g.Count(),
                    CreatedAtUtc: g.Min(x => x.CreatedAtUtc),
                    LastOrderAtUtc: latest.CreatedAtUtc);
            })
            .ToList();

        var registeredRows = users.Select(user =>
        {
            userOrderStats.TryGetValue(user.Id, out var orderStat);
            return new AdminUserListItemDto(
                Id: user.Id.ToString(),
                UserType: "Registered",
                FullName: user.FullName,
                Email: user.Email,
                Phone: user.Phone,
                Role: user.Role,
                IsActive: user.IsActive,
                CanToggleAccess: true,
                Address: orderStat?.LastAddress ?? string.Empty,
                Area: orderStat?.LastArea ?? string.Empty,
                OrdersCount: orderStat?.OrdersCount ?? 0,
                CreatedAtUtc: user.CreatedAtUtc,
                LastOrderAtUtc: orderStat?.LastOrderAtUtc);
        });

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            registeredRows = registeredRows.Where(x =>
                (!string.IsNullOrWhiteSpace(x.FullName) && x.FullName.Contains(term, StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrWhiteSpace(x.Email) && x.Email.Contains(term, StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrWhiteSpace(x.Phone) && x.Phone.Contains(term, StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrWhiteSpace(x.Role) && x.Role.Contains(term, StringComparison.OrdinalIgnoreCase)));

            guestRows = guestRows.Where(x =>
                (!string.IsNullOrWhiteSpace(x.FullName) && x.FullName.Contains(term, StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrWhiteSpace(x.Email) && x.Email.Contains(term, StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrWhiteSpace(x.Phone) && x.Phone.Contains(term, StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrWhiteSpace(x.Role) && x.Role.Contains(term, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        var rows = registeredRows
            .Concat(guestRows)
            .OrderByDescending(x => x.LastOrderAtUtc ?? x.CreatedAtUtc)
            .ThenByDescending(x => x.CreatedAtUtc)
            .ToList();

        return Ok(rows);
    }

    [HttpPatch("users/{id:guid}/access")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserAccess(Guid id, [FromBody] AdminUserAccessUpdateRequest request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                   ?? throw new AppNotFoundException("User not found");

        var currentUserId = User.GetUserId();
        if (currentUserId == id && !request.IsActive)
        {
            throw new AppValidationException("You cannot disable your own account");
        }

        user.IsActive = request.IsActive;
        user.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("reports/sales")]
    public async Task<IActionResult> Sales([FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken cancellationToken)
    {
        var orders = await dbContext.Orders
            .Where(x => x.CreatedAtUtc >= from && x.CreatedAtUtc <= to)
            .Include(x => x.OrderItems)
            .ToListAsync(cancellationToken);

        var topProducts = orders.SelectMany(o => o.OrderItems)
            .GroupBy(i => i.ProductNameSnapshot)
            .Select(g => new { Product = g.Key, Quantity = g.Sum(x => x.Quantity) })
            .OrderByDescending(x => x.Quantity)
            .Take(10)
            .ToList();

        return Ok(new
        {
            from,
            to,
            ordersCount = orders.Count,
            grossSales = orders.Sum(x => x.TotalAmount),
            paidSales = orders.Where(x => x.IsPaid).Sum(x => x.TotalAmount),
            topProducts
        });
    }

    [HttpGet("products")]
    public async Task<IActionResult> Products(CancellationToken cancellationToken)
    {
        var items = await dbContext.Products
            .Include(x => x.Category)
            .Include(x => x.Variants)
            .Include(x => x.Images)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Slug,
                x.Description,
                x.CategoryId,
                Category = x.Category!.Name,
                x.IsFeatured,
                x.IsActive,
                variants = x.Variants.Select(v => new { v.Id, v.Name, v.Price, v.IsDefault }),
                images = x.Images.Select(i => i.ImageUrl)
            })
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] Category category, CancellationToken cancellationToken)
    {
        category.Id = Guid.NewGuid();
        category.Slug = SlugHelper.Generate(category.Name);
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(category);
    }

    [HttpPut("categories/{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] Category update, CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppNotFoundException("Category not found");

        category.Name = update.Name;
        category.Description = update.Description;
        category.IsFeatured = update.IsFeatured;
        category.IsActive = update.IsActive;
        category.Slug = SlugHelper.Generate(update.Name);
        category.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(category);
    }

    [HttpDelete("categories/{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppNotFoundException("Category not found");

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct([FromBody] Product product, CancellationToken cancellationToken)
    {
        product.Id = Guid.NewGuid();
        product.Slug = SlugHelper.Generate(product.Name);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(product);
    }

    [HttpPut("products/{id:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] Product update, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppNotFoundException("Product not found");

        product.Name = update.Name;
        product.Description = update.Description;
        product.CategoryId = update.CategoryId;
        product.IsFeatured = update.IsFeatured;
        product.IsActive = update.IsActive;
        product.Slug = SlugHelper.Generate(update.Name);
        product.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(product);
    }

    [HttpDelete("products/{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppNotFoundException("Product not found");

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("products/{id:guid}/config")]
    public async Task<IActionResult> ProductConfig(Guid id, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(x => x.Variants)
            .Include(x => x.ProductAddOns)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppNotFoundException("Product not found");

        var addOns = await dbContext.AddOns
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new { x.Id, x.Name, x.Price })
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            product.Id,
            product.Name,
            variants = product.Variants
                .OrderByDescending(x => x.IsDefault)
                .ThenBy(x => x.Price)
                .Select(x => new { x.Id, x.Name, x.Price, x.IsDefault, x.IsActive }),
            selectedAddOnIds = product.ProductAddOns.Select(x => x.AddOnId),
            availableAddOns = addOns
        });
    }

    [HttpPut("products/{id:guid}/config")]
    public async Task<IActionResult> UpdateProductConfig(Guid id, ProductConfigUpdateRequest request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(x => x.Variants)
            .Include(x => x.ProductAddOns)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppNotFoundException("Product not found");

        if (request.Variants.Count == 0)
        {
            throw new AppValidationException("At least one variant is required");
        }

        var incomingIds = request.Variants.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();

        foreach (var variantInput in request.Variants)
        {
            ProductVariant? variant = null;
            if (variantInput.Id.HasValue)
            {
                variant = product.Variants.FirstOrDefault(x => x.Id == variantInput.Id.Value);
            }

            if (variant is null)
            {
                variant = new ProductVariant
                {
                    Id = Guid.NewGuid(),
                    ProductId = id,
                    CreatedAtUtc = DateTime.UtcNow
                };
                dbContext.ProductVariants.Add(variant);
            }

            variant.Name = variantInput.Name.Trim();
            variant.Price = variantInput.Price;
            variant.IsDefault = variantInput.IsDefault;
            variant.IsActive = variantInput.IsActive;
            variant.UpdatedAtUtc = DateTime.UtcNow;
        }

        foreach (var existing in product.Variants.Where(x => !incomingIds.Contains(x.Id)))
        {
            existing.IsActive = false;
            existing.IsDefault = false;
            existing.UpdatedAtUtc = DateTime.UtcNow;
        }

        var activeVariants = product.Variants.Where(x => x.IsActive).ToList();
        if (activeVariants.Count > 0 && activeVariants.All(x => !x.IsDefault))
        {
            activeVariants[0].IsDefault = true;
        }

        dbContext.ProductAddOns.RemoveRange(product.ProductAddOns);
        foreach (var addOnId in request.AddOnIds.Distinct())
        {
            dbContext.ProductAddOns.Add(new ProductAddOn
            {
                ProductId = id,
                AddOnId = addOnId
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("addons")]
    public async Task<IActionResult> CreateAddOn([FromBody] AddOn addOn, CancellationToken cancellationToken)
    {
        addOn.Id = Guid.NewGuid();
        dbContext.AddOns.Add(addOn);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(addOn);
    }

    [HttpGet("addons")]
    public async Task<IActionResult> AddOns(CancellationToken cancellationToken)
    {
        return Ok(await dbContext.AddOns.OrderBy(x => x.Name).ToListAsync(cancellationToken));
    }

    [HttpPost("delivery-zones")]
    public async Task<IActionResult> CreateZone([FromBody] DeliveryZone zone, CancellationToken cancellationToken)
    {
        zone.Id = Guid.NewGuid();
        dbContext.DeliveryZones.Add(zone);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(zone);
    }

    [HttpGet("delivery-zones")]
    public async Task<IActionResult> Zones(CancellationToken cancellationToken)
    {
        return Ok(await dbContext.DeliveryZones.OrderBy(x => x.Emirate).ThenBy(x => x.Area).ToListAsync(cancellationToken));
    }

    [HttpPut("delivery-zones/{id:guid}")]
    public async Task<IActionResult> UpdateZone(Guid id, [FromBody] DeliveryZone update, CancellationToken cancellationToken)
    {
        var zone = await dbContext.DeliveryZones.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppNotFoundException("Delivery zone not found");

        zone.Emirate = update.Emirate;
        zone.Area = update.Area;
        zone.Charge = update.Charge;
        zone.IsActive = update.IsActive;
        zone.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(zone);
    }

    [HttpDelete("delivery-zones/{id:guid}")]
    public async Task<IActionResult> DeleteZone(Guid id, CancellationToken cancellationToken)
    {
        var zone = await dbContext.DeliveryZones.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppNotFoundException("Delivery zone not found");

        dbContext.DeliveryZones.Remove(zone);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("delivery-slots")]
    public async Task<IActionResult> CreateSlot([FromBody] DeliveryTimeSlot slot, CancellationToken cancellationToken)
    {
        slot.Id = Guid.NewGuid();
        dbContext.DeliveryTimeSlots.Add(slot);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(slot);
    }

    [HttpGet("delivery-slots")]
    public async Task<IActionResult> Slots(CancellationToken cancellationToken)
    {
        return Ok(await dbContext.DeliveryTimeSlots.OrderBy(x => x.StartTime).ToListAsync(cancellationToken));
    }

    [HttpPut("delivery-slots/{id:guid}")]
    public async Task<IActionResult> UpdateSlot(Guid id, [FromBody] DeliveryTimeSlot update, CancellationToken cancellationToken)
    {
        var slot = await dbContext.DeliveryTimeSlots.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppNotFoundException("Delivery slot not found");

        slot.Label = update.Label;
        slot.StartTime = update.StartTime;
        slot.EndTime = update.EndTime;
        slot.CapacityLimit = update.CapacityLimit;
        slot.IsActive = update.IsActive;
        slot.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(slot);
    }

    [HttpDelete("delivery-slots/{id:guid}")]
    public async Task<IActionResult> DeleteSlot(Guid id, CancellationToken cancellationToken)
    {
        var slot = await dbContext.DeliveryTimeSlots.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppNotFoundException("Delivery slot not found");

        dbContext.DeliveryTimeSlots.Remove(slot);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("delivery-settings")]
    public async Task<IActionResult> DeliverySettings(CancellationToken cancellationToken)
    {
        var cutoff = await dbContext.AppSettings.FirstOrDefaultAsync(x => x.Key == "SameDayCutoffHour", cancellationToken);
        var hour = cutoff is null ? 14 : int.Parse(cutoff.Value);
        return Ok(new { sameDayCutoffHour = hour });
    }

    [HttpPut("delivery-settings")]
    public async Task<IActionResult> UpdateDeliverySettings([FromBody] DeliverySettingsUpdateRequest request, CancellationToken cancellationToken)
    {
        if (request.SameDayCutoffHour is < 0 or > 23)
        {
            throw new AppValidationException("Cutoff hour must be between 0 and 23");
        }

        var cutoff = await dbContext.AppSettings.FirstOrDefaultAsync(x => x.Key == "SameDayCutoffHour", cancellationToken);
        if (cutoff is null)
        {
            cutoff = new AppSetting
            {
                Id = Guid.NewGuid(),
                Key = "SameDayCutoffHour",
                Value = request.SameDayCutoffHour.ToString()
            };
            dbContext.AppSettings.Add(cutoff);
        }
        else
        {
            cutoff.Value = request.SameDayCutoffHour.ToString();
            cutoff.UpdatedAtUtc = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("promocodes")]
    public async Task<IActionResult> CreatePromo([FromBody] PromoCode promo, CancellationToken cancellationToken)
    {
        promo.Id = Guid.NewGuid();
        promo.Code = promo.Code.ToUpperInvariant();
        dbContext.PromoCodes.Add(promo);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(promo);
    }

    [HttpGet("promocodes")]
    public async Task<IActionResult> Promos(CancellationToken cancellationToken)
    {
        return Ok(await dbContext.PromoCodes.OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken));
    }

    [HttpPut("promocodes/{id:guid}")]
    public async Task<IActionResult> UpdatePromo(Guid id, [FromBody] PromoCode update, CancellationToken cancellationToken)
    {
        var promo = await dbContext.PromoCodes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppNotFoundException("Promo code not found");

        promo.Code = update.Code.ToUpperInvariant();
        promo.Type = update.Type;
        promo.Value = update.Value;
        promo.MinSpend = update.MinSpend;
        promo.ExpiresAtUtc = update.ExpiresAtUtc;
        promo.UsageLimit = update.UsageLimit;
        promo.IsActive = update.IsActive;
        promo.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(promo);
    }

    [HttpDelete("promocodes/{id:guid}")]
    public async Task<IActionResult> DeletePromo(Guid id, CancellationToken cancellationToken)
    {
        var promo = await dbContext.PromoCodes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppNotFoundException("Promo code not found");

        dbContext.PromoCodes.Remove(promo);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("products/{productId:guid}/upload-image")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> UploadImage(Guid productId, IFormFile file, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == productId, cancellationToken)
            ?? throw new AppNotFoundException("Product not found");

        await using var stream = file.OpenReadStream();
        var url = await blobStorageService.UploadAsync(stream, file.FileName, file.ContentType, cancellationToken);

        var image = new ProductImage
        {
            ProductId = product.Id,
            ImageUrl = url,
            AltText = product.Name,
            SortOrder = 1
        };

        dbContext.ProductImages.Add(image);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(image);
    }
}

