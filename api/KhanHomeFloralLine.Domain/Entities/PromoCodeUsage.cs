using KhanHomeFloralLine.Domain.Common;

namespace KhanHomeFloralLine.Domain.Entities;

public class PromoCodeUsage : BaseEntity
{
    public Guid PromoCodeId { get; set; }
    public Guid? UserId { get; set; }
    public Guid OrderId { get; set; }
    public DateTime UsedAtUtc { get; set; }

    public PromoCode? PromoCode { get; set; }
    public User? User { get; set; }
    public Order? Order { get; set; }
}

