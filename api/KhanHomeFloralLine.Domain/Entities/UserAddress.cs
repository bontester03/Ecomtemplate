using KhanHomeFloralLine.Domain.Common;

namespace KhanHomeFloralLine.Domain.Entities;

public class UserAddress : BaseEntity
{
    public Guid UserId { get; set; }
    public string Label { get; set; } = "Address";
    public string RecipientName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Emirate { get; set; } = string.Empty;
    public string? Landmark { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;

    public User? User { get; set; }
}

