using KhanHomeFloralLine.Application.Abstractions;

namespace KhanHomeFloralLine.Infrastructure.Services;

public class OrderNumberGenerator : IOrderNumberGenerator
{
    public string Generate()
    {
        return $"KHF-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
    }
}

