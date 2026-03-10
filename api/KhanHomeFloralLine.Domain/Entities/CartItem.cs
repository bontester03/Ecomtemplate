using KhanHomeFloralLine.Domain.Common;

namespace KhanHomeFloralLine.Domain.Entities;

public class CartItem : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public string? MessageCard { get; set; }

    public User? User { get; set; }
    public Product? Product { get; set; }
    public ProductVariant? ProductVariant { get; set; }
    public ICollection<CartItemAddOn> AddOns { get; set; } = new List<CartItemAddOn>();
}

