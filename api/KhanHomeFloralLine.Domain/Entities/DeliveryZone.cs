using KhanHomeFloralLine.Domain.Common;

namespace KhanHomeFloralLine.Domain.Entities;

public class DeliveryZone : BaseEntity
{
    public string Emirate { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public decimal Charge { get; set; }
    public bool IsActive { get; set; } = true;
}

