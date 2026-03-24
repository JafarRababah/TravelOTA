using Microsoft.AspNetCore.Mvc;
using TravelOTA.Application;
using TravelOTA.Entitis.Domain;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentsController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] AmadeusPrice dto)
    {
        var paymentIntent = await _paymentService
            .CreatePaymentIntent(dto.Total, dto.Currency);

        return Ok(new
        {
            clientSecret = paymentIntent.ClientSecret
        });
    }
}