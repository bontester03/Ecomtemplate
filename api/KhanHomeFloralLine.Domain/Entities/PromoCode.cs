using KhanHomeFloralLine.Domain.Common;
using KhanHomeFloralLine.Domain.Enums;

namespace KhanHomeFloralLine.Domain.Entities;

public class PromoCode : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public PromoType Type { get; set; }
    public decimal Value { get; set; }
    public decimal MinSpend { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public int? UsageLimit { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<PromoCodeUsage> Usages { get; set; } = new List<PromoCodeUsage>();
}

