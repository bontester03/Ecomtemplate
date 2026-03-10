using KhanHomeFloralLine.Application.Payments;

namespace KhanHomeFloralLine.Application.Abstractions;

public interface IPaymentGatewayService
{
    Task<PaymentInitiationResult> InitiateAsync(PaymentInitiationRequest request, CancellationToken cancellationToken = default);
    Task<bool> HandleWebhookAsync(PaymentWebhookPayload payload, CancellationToken cancellationToken = default);
}

