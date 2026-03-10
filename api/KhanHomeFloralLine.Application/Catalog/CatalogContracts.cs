namespace KhanHomeFloralLine.Application.Catalog;

public sealed record ProductFilterRequest(string? Search, Guid? CategoryId, bool? FeaturedOnly, string? SortBy, int Page = 1, int PageSize = 20);

public sealed record ProductListItemDto(Guid Id, string Name, string Slug, string CategoryName, decimal StartingPrice, string? ImageUrl, bool IsFeatured);

public sealed record ProductDetailDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    string CategoryName,
    IReadOnlyCollection<ProductVariantDto> Variants,
    IReadOnlyCollection<AddOnDto> AddOns,
    IReadOnlyCollection<string> ImageUrls);

public sealed record ProductVariantDto(Guid Id, string Name, decimal Price, bool IsDefault);
public sealed record AddOnDto(Guid Id, string Name, decimal Price);

public sealed record CategoryDto(Guid Id, string Name, string Slug, bool IsFeatured);

