using KhanHomeFloralLine.Domain.Common;

namespace KhanHomeFloralLine.Domain.Entities;

public class ProductImage : BaseEntity
{
    public Guid ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int SortOrder { get; set; }

    public Product? Product { get; set; }
}

