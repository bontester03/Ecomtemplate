using KhanHomeFloralLine.Domain.Common;

namespace KhanHomeFloralLine.Domain.Entities;

public class DeliveryTimeSlot : BaseEntity
{
    public string Label { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int? CapacityLimit { get; set; }
    public bool IsActive { get; set; } = true;
}

