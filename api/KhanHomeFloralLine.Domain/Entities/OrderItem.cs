using KhanHomeFloralLine.Domain.Common;

namespace KhanHomeFloralLine.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductVariantId { get; set; }
    public string ProductNameSnapshot { get; set; } = string.Empty;
    public string VariantNameSnapshot { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public Order? Order { get; set; }
    public Product? Product { get; set; }
    public ProductVariant? ProductVariant { get; set; }
    public ICollection<OrderItemAddOn> AddOns { get; set; } = new List<OrderItemAddOn>();
}

