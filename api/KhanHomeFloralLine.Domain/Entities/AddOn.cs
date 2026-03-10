using KhanHomeFloralLine.Domain.Common;

namespace KhanHomeFloralLine.Domain.Entities;

public class AddOn : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ProductAddOn> ProductAddOns { get; set; } = new List<ProductAddOn>();
}

