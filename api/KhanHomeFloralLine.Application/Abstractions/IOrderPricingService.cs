namespace KhanHomeFloralLine.Application.Abstractions;

public interface IOrderPricingService
{
    decimal CalculateSubtotal(IReadOnlyCollection<(decimal UnitPrice, int Quantity, decimal AddOnTotal)> items);
    decimal CalculateTotal(decimal subtotal, decimal discount, decimal deliveryCharge);
}

