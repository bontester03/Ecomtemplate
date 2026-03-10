using KhanHomeFloralLine.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KhanHomeFloralLine.Infrastructure.Persistence;

public sealed class CatalogDataSeeder(AppDbContext dbContext)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var categories = await EnsureCategoriesAsync(cancellationToken);
        var addOns = await EnsureAddOnsAsync(cancellationToken);
        await EnsureProductsAsync(categories, addOns, cancellationToken);
    }

    private async Task<Dictionary<string, Category>> EnsureCategoriesAsync(CancellationToken cancellationToken)
    {
        var rows = new (string Name, string Slug, bool IsFeatured)[]
        {
            ("Our Complete Range", "our-complete-range", true),
            ("Valentines Day Flowers", "valentines-day-flowers", true),
            ("Birthday Flowers", "birthday-flowers", true),
            ("Anniversary Flowers", "anniversary-flowers", true),
            ("VIP Flowers", "vip-flowers", true),
            ("Rose Collection", "rose-collection", true),
            ("Occasions", "occasions", true),
            ("Bouquets", "bouquets", true),
            ("Add-ons", "add-ons", false),
            ("Luxury Collections", "luxury-collections", false)
        };

        var categories = new Dictionary<string, Category>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in rows)
        {
            var category = await dbContext.Categories.FirstOrDefaultAsync(x => x.Slug == row.Slug, cancellationToken);
            if (category is null)
            {
                category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = row.Name,
                    Slug = row.Slug,
                    IsFeatured = row.IsFeatured,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                };
                dbContext.Categories.Add(category);
            }
            else
            {
                category.Name = row.Name;
                category.IsFeatured = row.IsFeatured;
                category.IsActive = true;
                category.UpdatedAtUtc = DateTime.UtcNow;
            }

            categories[row.Slug] = category;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return categories;
    }

    private async Task<Dictionary<string, AddOn>> EnsureAddOnsAsync(CancellationToken cancellationToken)
    {
        var rows = new (string Name, decimal Price)[]
        {
            ("Belgian Chocolate Box", 45m),
            ("Celebration Balloons", 30m),
            ("Greeting Teddy Bear", 55m),
            ("Premium Scented Candle", 65m)
        };

        var addOns = new Dictionary<string, AddOn>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in rows)
        {
            var addOn = await dbContext.AddOns.FirstOrDefaultAsync(x => x.Name == row.Name, cancellationToken);
            if (addOn is null)
            {
                addOn = new AddOn
                {
                    Id = Guid.NewGuid(),
                    Name = row.Name,
                    Price = row.Price,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                };
                dbContext.AddOns.Add(addOn);
            }
            else
            {
                addOn.Price = row.Price;
                addOn.IsActive = true;
                addOn.UpdatedAtUtc = DateTime.UtcNow;
            }

            addOns[row.Name] = addOn;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return addOns;
    }

    private async Task EnsureProductsAsync(
        IReadOnlyDictionary<string, Category> categories,
        IReadOnlyDictionary<string, AddOn> addOns,
        CancellationToken cancellationToken)
    {
        var products = new ProductSeed[]
        {
            new("royal-red-roses", "Royal Red Roses", "Premium red rose arrangement.", "rose-collection", "/assets/images/product-1.svg", 199m, 279m, 359m,
                "Belgian Chocolate Box", "Celebration Balloons"),
            new("grand-white-elegance", "Grand White Elegance", "Elegant white lily and rose arrangement.", "vip-flowers", "/assets/images/product-3.svg", 289m, 349m, 419m,
                "Greeting Teddy Bear", "Premium Scented Candle"),
            new("pastel-love-mix", "Pastel Love Mix", "Soft pastel bouquet for special occasions.", "anniversary-flowers", "/assets/images/product-2.svg", 249m, 299m, 369m,
                "Belgian Chocolate Box", "Greeting Teddy Bear"),
            new("sunset-celebration-box", "Sunset Celebration Box", "Warm-toned floral box for birthday surprises.", "birthday-flowers", "/assets/images/product-4.svg", 219m, 289m, 349m,
                "Celebration Balloons", "Belgian Chocolate Box"),
            new("velvet-orchid-luxe", "Velvet Orchid Luxe", "Luxury orchid arrangement with premium wrapping.", "vip-flowers", "/assets/images/product-5.svg", 329m, 399m, 479m,
                "Premium Scented Candle", "Greeting Teddy Bear"),
            new("soft-blush-harmony", "Soft Blush Harmony", "A gentle blush bouquet for heartfelt occasions.", "our-complete-range", "/assets/images/product-6.svg", 189m, 249m, 319m,
                "Belgian Chocolate Box"),
            new("crimson-signature-vase", "Crimson Signature Vase", "Bold crimson arrangement in a keepsake vase.", "valentines-day-flowers", "/assets/images/product-7.svg", 269m, 329m, 399m,
                "Premium Scented Candle", "Celebration Balloons"),
            new("moonlight-lily-grace", "Moonlight Lily Grace", "Moonlit lily bouquet with elegant accents.", "our-complete-range", "/assets/images/product-8.svg", 209m, 279m, 349m,
                "Greeting Teddy Bear")
        };

        foreach (var seed in products)
        {
            if (!categories.TryGetValue(seed.CategorySlug, out var category))
            {
                continue;
            }

            var product = await dbContext.Products
                .Include(x => x.Images)
                .Include(x => x.Variants)
                .Include(x => x.ProductAddOns)
                .FirstOrDefaultAsync(x => x.Slug == seed.Slug, cancellationToken);

            if (product is null)
            {
                product = new Product
                {
                    Id = Guid.NewGuid(),
                    CategoryId = category.Id,
                    Name = seed.Name,
                    Slug = seed.Slug,
                    Description = seed.Description,
                    IsFeatured = true,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                };
                dbContext.Products.Add(product);
            }
            else
            {
                product.CategoryId = category.Id;
                product.Name = seed.Name;
                product.Description = seed.Description;
                product.IsFeatured = true;
                product.IsActive = true;
                product.UpdatedAtUtc = DateTime.UtcNow;
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            await EnsurePrimaryImageAsync(product, seed.ImageUrl, cancellationToken);
            await EnsureVariantAsync(product.Id, "Small", seed.SmallPrice, true, cancellationToken);
            await EnsureVariantAsync(product.Id, "Medium", seed.MediumPrice, false, cancellationToken);
            await EnsureVariantAsync(product.Id, "Large", seed.LargePrice, false, cancellationToken);
            await EnsureProductAddOnsAsync(product, seed.AddOnNames, addOns, cancellationToken);
        }
    }

    private async Task EnsurePrimaryImageAsync(Product product, string imageUrl, CancellationToken cancellationToken)
    {
        var image = await dbContext.ProductImages.FirstOrDefaultAsync(x => x.ProductId == product.Id && x.SortOrder == 1, cancellationToken);
        if (image is null)
        {
            image = new ProductImage
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                ImageUrl = imageUrl,
                SortOrder = 1,
                CreatedAtUtc = DateTime.UtcNow
            };
            dbContext.ProductImages.Add(image);
        }
        else
        {
            image.ImageUrl = imageUrl;
            image.UpdatedAtUtc = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureVariantAsync(Guid productId, string variantName, decimal price, bool isDefault, CancellationToken cancellationToken)
    {
        var variant = await dbContext.ProductVariants.FirstOrDefaultAsync(
            x => x.ProductId == productId && x.Name.ToLower() == variantName.ToLower(),
            cancellationToken);

        if (variant is null)
        {
            variant = new ProductVariant
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Name = variantName,
                Price = price,
                IsDefault = isDefault,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };
            dbContext.ProductVariants.Add(variant);
        }
        else
        {
            variant.Price = price;
            variant.IsDefault = isDefault;
            variant.IsActive = true;
            variant.UpdatedAtUtc = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureProductAddOnsAsync(
        Product product,
        IReadOnlyCollection<string> addOnNames,
        IReadOnlyDictionary<string, AddOn> addOns,
        CancellationToken cancellationToken)
    {
        var selectedAddOnIds = addOnNames
            .Where(addOns.ContainsKey)
            .Select(name => addOns[name].Id)
            .ToHashSet();

        var existing = await dbContext.ProductAddOns
            .Where(x => x.ProductId == product.Id)
            .ToListAsync(cancellationToken);

        var toRemove = existing.Where(x => !selectedAddOnIds.Contains(x.AddOnId)).ToList();
        if (toRemove.Count > 0)
        {
            dbContext.ProductAddOns.RemoveRange(toRemove);
        }

        foreach (var addOnId in selectedAddOnIds)
        {
            var exists = existing.Any(x => x.AddOnId == addOnId);
            if (exists) continue;

            dbContext.ProductAddOns.Add(new ProductAddOn
            {
                ProductId = product.Id,
                AddOnId = addOnId
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private sealed record ProductSeed(
        string Slug,
        string Name,
        string Description,
        string CategorySlug,
        string ImageUrl,
        decimal SmallPrice,
        decimal MediumPrice,
        decimal LargePrice,
        params string[] AddOnNames);
}
