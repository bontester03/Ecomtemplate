using KhanHomeFloralLine.Application.Delivery;

namespace KhanHomeFloralLine.Application.Abstractions;

public interface IDeliveryService
{
    Task<DeliveryValidationResult> ValidateAsync(DeliveryValidationRequest request, CancellationToken cancellationToken = default);
}

