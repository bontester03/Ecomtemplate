namespace KhanHomeFloralLine.Domain.Entities;

public class ProductAddOn
{
    public Guid ProductId { get; set; }
    public Guid AddOnId { get; set; }

    public Product? Product { get; set; }
    public AddOn? AddOn { get; set; }
}

