using KhanHomeFloralLine.Domain.Common;

namespace KhanHomeFloralLine.Domain.Entities;

public class OrderItemAddOn : BaseEntity
{
    public Guid OrderItemId { get; set; }
    public Guid AddOnId { get; set; }
    public string AddOnNameSnapshot { get; set; } = string.Empty;
    public decimal PriceSnapshot { get; set; }

    public OrderItem? OrderItem { get; set; }
    public AddOn? AddOn { get; set; }
}

