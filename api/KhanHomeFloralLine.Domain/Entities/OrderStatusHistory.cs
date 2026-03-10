using KhanHomeFloralLine.Domain.Common;
using KhanHomeFloralLine.Domain.Enums;

namespace KhanHomeFloralLine.Domain.Entities;

public class OrderStatusHistory : BaseEntity
{
    public Guid OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public string? Note { get; set; }

    public Order? Order { get; set; }
}

