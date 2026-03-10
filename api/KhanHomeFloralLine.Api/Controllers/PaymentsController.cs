using System.Text.Json;
using KhanHomeFloralLine.Api.Models;
using KhanHomeFloralLine.Application.Abstractions;
using KhanHomeFloralLine.Application.Common;
using KhanHomeFloralLine.Application.Payments;
using KhanHomeFloralLine.Domain.Entities;
using KhanHomeFloralLine.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhanHomeFloralLine.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController(IAppDbContext dbContext, IPaymentGatewayService paymentGatewayService) : ControllerBase
{
    [HttpPost("initiate")]
    [Authorize]
    public async Task<ActionResult<PaymentInitiationResult>> Initiate(InitiatePaymentApiRequest request, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken)
                    ?? throw new AppNotFoundException("Order not found");

        var result = await paymentGatewayService.InitiateAsync(new PaymentInitiationRequest(
            order.Id,
            order.TotalAmount,
            "AED",
            request.ReturnUrl,
            request.CancelUrl), cancellationToken);

        dbContext.Payments.Add(new Payment
        {
            OrderId = order.Id,
            Gateway = result.Gateway,
            GatewayReference = result.CheckoutToken,
            Amount = order.TotalAmount,
            Status = PaymentStatus.Pending
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost("webhook/mock")]
    [AllowAnonymous]
    public async Task<IActionResult> MockWebhook(PaymentWebhookApiRequest request, CancellationToken cancellationToken)
    {
        var handled = await paymentGatewayService.HandleWebhookAsync(new PaymentWebhookPayload(
            request.GatewayReference,
            request.Status,
            request.Amount,
            JsonSerializer.Serialize(request)), cancellationToken);

        var payment = await dbContext.Payments.Include(x => x.Order)
            .FirstOrDefaultAsync(x => x.GatewayReference == request.GatewayReference, cancellationToken);

        if (payment is null || payment.Order is null)
        {
            throw new AppNotFoundException("Payment not found");
        }

        payment.RawResponse = JsonSerializer.Serialize(request);
        payment.Status = handled ? PaymentStatus.Paid : PaymentStatus.Failed;
        payment.UpdatedAtUtc = DateTime.UtcNow;

        if (handled)
        {
            payment.Order.IsPaid = true;
            payment.Order.UpdatedAtUtc = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(new { handled });
    }

    // Placeholders for real gateway integrations:
    // [HttpPost("webhook/telr")] public Task<IActionResult> TelrWebhook(...) { ... }
    // [HttpPost("webhook/paytabs")] public Task<IActionResult> PayTabsWebhook(...) { ... }
    // [HttpPost("webhook/stripe")] public Task<IActionResult> StripeWebhook(...) { ... }
}

