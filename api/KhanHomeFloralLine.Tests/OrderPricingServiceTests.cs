using KhanHomeFloralLine.Application.Orders;

namespace KhanHomeFloralLine.Tests;

public class OrderPricingServiceTests
{
    private readonly OrderPricingService _service = new();

    [Fact]
    public void CalculateSubtotal_ShouldIncludeVariantsAndAddOns()
    {
        var subtotal = _service.CalculateSubtotal([
            (100m, 2, 20m),
            (250m, 1, 0m)
        ]);

        Assert.Equal(490m, subtotal);
    }

    [Fact]
    public void CalculateTotal_ShouldNotGoBelowZero()
    {
        var total = _service.CalculateTotal(120m, 200m, 0m);
        Assert.Equal(0m, total);
    }
}

