using KhanHomeFloralLine.Application.Abstractions;
using KhanHomeFloralLine.Application.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhanHomeFloralLine.Api.Controllers;

[ApiController]
[Route("api/catalog")]
public class CatalogController(IAppDbContext dbContext) : ControllerBase
{
    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyCollection<CategoryDto>>> Categories(CancellationToken cancellationToken)
    {
        var categories = await dbContext.Categories
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.IsFeatured)
            .ThenBy(x => x.Name)
            .Select(x => new CategoryDto(x.Id, x.Name, x.Slug, x.IsFeatured))
            .ToListAsync(cancellationToken);

        return Ok(categories);
    }

    [HttpGet("products")]
    public async Task<ActionResult<IReadOnlyCollection<ProductListItemDto>>> Products([FromQuery] ProductFilterRequest request, CancellationToken cancellationToken)
    {
        var query = dbContext.Products
            .Where(x => x.IsActive)
            .Include(x => x.Category)
            .Include(x => x.Variants.Where(v => v.IsActive))
            .Include(x => x.Images)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(search) || x.Description.ToLower().Contains(search));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == request.CategoryId.Value);
        }

        if (request.FeaturedOnly == true)
        {
            query = query.Where(x => x.IsFeatured);
        }

        query = request.SortBy?.ToLower() switch
        {
            "price_asc" => query.OrderBy(x => x.Variants.Min(v => v.Price)),
            "price_desc" => query.OrderByDescending(x => x.Variants.Min(v => v.Price)),
            _ => query.OrderByDescending(x => x.IsFeatured).ThenBy(x => x.Name)
        };

        var skip = (Math.Max(request.Page, 1) - 1) * Math.Clamp(request.PageSize, 1, 100);
        var take = Math.Clamp(request.PageSize, 1, 100);

        var items = await query.Skip(skip).Take(take)
            .Select(x => new ProductListItemDto(
                x.Id,
                x.Name,
                x.Slug,
                x.Category!.Name,
                x.Variants.Where(v => v.IsActive).OrderBy(v => v.Price).Select(v => v.Price).FirstOrDefault(),
                x.Images.OrderBy(i => i.SortOrder).Select(i => i.ImageUrl).FirstOrDefault(),
                x.IsFeatured))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("products/{slug}")]
    public async Task<ActionResult<ProductDetailDto>> ProductDetail(string slug, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(x => x.Category)
            .Include(x => x.Variants.Where(v => v.IsActive))
            .Include(x => x.Images)
            .Include(x => x.ProductAddOns)
            .ThenInclude(pa => pa.AddOn)
            .FirstOrDefaultAsync(x => x.Slug == slug && x.IsActive, cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        var dto = new ProductDetailDto(
            product.Id,
            product.Name,
            product.Slug,
            product.Description,
            product.Category?.Name ?? string.Empty,
            product.Variants.OrderBy(v => v.Price).Select(v => new ProductVariantDto(v.Id, v.Name, v.Price, v.IsDefault)).ToList(),
            product.ProductAddOns.Where(x => x.AddOn != null && x.AddOn.IsActive).Select(x => new AddOnDto(x.AddOnId, x.AddOn!.Name, x.AddOn.Price)).ToList(),
            product.Images.OrderBy(x => x.SortOrder).Select(x => x.ImageUrl).ToList());

        return Ok(dto);
    }
}

