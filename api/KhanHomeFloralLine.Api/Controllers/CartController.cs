using KhanHomeFloralLine.Api.Extensions;
using KhanHomeFloralLine.Application.Abstractions;
using KhanHomeFloralLine.Application.Common;
using KhanHomeFloralLine.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhanHomeFloralLine.Api.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController(IAppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId() ?? throw new AppValidationException("Missing user id");

        var items = await dbContext.CartItems
            .Where(x => x.UserId == userId)
            .Include(x => x.Product)
            .Include(x => x.ProductVariant)
            .Include(x => x.AddOns)
            .ThenInclude(x => x.AddOn)
            .ToListAsync(cancellationToken);

        var result = items.Select(x => new
        {
            x.Id,
            x.ProductId,
            ProductName = x.Product!.Name,
            x.ProductVariantId,
            VariantName = x.ProductVariant!.Name,
            VariantPrice = x.ProductVariant!.Price,
            x.Quantity,
            x.MessageCard,
            addOns = x.AddOns.Select(a => new { a.AddOnId, Name = a.AddOn!.Name, Price = a.AddOn!.Price })
        });

        return Ok(result);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] CartItem item, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId() ?? throw new AppValidationException("Missing user id");

        item.Id = Guid.NewGuid();
        item.UserId = userId;
        item.MessageCard = item.MessageCard?.Trim();

        dbContext.CartItems.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(item);
    }

    [HttpDelete("items/{id:guid}")]
    public async Task<IActionResult> RemoveItem(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId() ?? throw new AppValidationException("Missing user id");

        var item = await dbContext.CartItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken)
            ?? throw new AppNotFoundException("Cart item not found");

        dbContext.CartItems.Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPatch("items/{id:guid}/quantity")]
    public async Task<IActionResult> UpdateQuantity(Guid id, [FromBody] int quantity, CancellationToken cancellationToken)
    {
        if (quantity < 1)
        {
            throw new AppValidationException("Quantity must be at least 1");
        }

        var userId = User.GetUserId() ?? throw new AppValidationException("Missing user id");

        var item = await dbContext.CartItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken)
            ?? throw new AppNotFoundException("Cart item not found");

        item.Quantity = quantity;
        item.UpdatedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(item);
    }
}

