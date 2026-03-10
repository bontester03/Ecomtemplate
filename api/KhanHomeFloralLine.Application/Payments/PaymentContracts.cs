namespace KhanHomeFloralLine.Application.Payments;

public sealed record PaymentInitiationRequest(Guid OrderId, decimal Amount, string Currency, string ReturnUrl, string CancelUrl);
public sealed record PaymentInitiationResult(string Gateway, string RedirectUrl, string CheckoutToken);
public sealed record PaymentWebhookPayload(string GatewayReference, string Status, decimal Amount, string RawBody);

