using KhanHomeFloralLine.Domain.Common;
using KhanHomeFloralLine.Domain.Enums;

namespace KhanHomeFloralLine.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    public string Gateway { get; set; } = string.Empty;
    public string GatewayReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? RawResponse { get; set; }

    public Order? Order { get; set; }
}

