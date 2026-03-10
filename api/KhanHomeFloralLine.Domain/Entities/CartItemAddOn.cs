using KhanHomeFloralLine.Domain.Common;

namespace KhanHomeFloralLine.Domain.Entities;

public class CartItemAddOn : BaseEntity
{
    public Guid CartItemId { get; set; }
    public Guid AddOnId { get; set; }

    public CartItem? CartItem { get; set; }
    public AddOn? AddOn { get; set; }
}

