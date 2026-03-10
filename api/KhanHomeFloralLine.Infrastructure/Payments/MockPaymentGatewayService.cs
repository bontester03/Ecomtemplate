using KhanHomeFloralLine.Application.Abstractions;
using KhanHomeFloralLine.Application.Payments;

namespace KhanHomeFloralLine.Infrastructure.Payments;

public class MockPaymentGatewayService : IPaymentGatewayService
{
    public Task<PaymentInitiationResult> InitiateAsync(PaymentInitiationRequest request, CancellationToken cancellationToken = default)
    {
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var redirectUrl = $"https://mock-gateway.local/checkout?token={Uri.EscapeDataString(token)}&orderId={request.OrderId}";
        return Task.FromResult(new PaymentInitiationResult("MockGateway", redirectUrl, token));
    }

    public Task<bool> HandleWebhookAsync(PaymentWebhookPayload payload, CancellationToken cancellationToken = default)
    {
        var success = payload.Status.Equals("paid", StringComparison.OrdinalIgnoreCase)
            || payload.Status.Equals("authorized", StringComparison.OrdinalIgnoreCase);

        return Task.FromResult(success);
    }
}

