using KhanHomeFloralLine.Application.Abstractions;

namespace KhanHomeFloralLine.Application.Orders;

public class OrderPricingService : IOrderPricingService
{
    public decimal CalculateSubtotal(IReadOnlyCollection<(decimal UnitPrice, int Quantity, decimal AddOnTotal)> items)
    {
        return items.Sum(i => (i.UnitPrice + i.AddOnTotal) * i.Quantity);
    }

    public decimal CalculateTotal(decimal subtotal, decimal discount, decimal deliveryCharge)
    {
        var total = subtotal - discount + deliveryCharge;
        return total < 0 ? 0 : decimal.Round(total, 2, MidpointRounding.AwayFromZero);
    }
}

