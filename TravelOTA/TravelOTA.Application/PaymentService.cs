//using Microsoft.Extensions.Configuration;
//using Stripe;

//public class PaymentService
//{
//    private readonly IConfiguration _config;

//    public PaymentService(IConfiguration config)
//    {
//        _config = config;
//        StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
//    }

//    public async Task<PaymentIntent> CreatePaymentIntent(
//        decimal amount,
//        string currency,
//        int bookingId)
//    {
//        var options = new PaymentIntentCreateOptions
//        {
//            Amount = (long)(amount * 100), // cents
//            Currency = currency,
//            PaymentMethodTypes = new List<string> { "card" },

//            // 🔥 ربط الدفع بالحجز
//            Metadata = new Dictionary<string, string>
//            {
//                { "bookingId", bookingId.ToString() }
//            }
//        };

//        var requestOptions = new RequestOptions
//        {
//            IdempotencyKey = Guid.NewGuid().ToString()
//        };

//        var service = new PaymentIntentService();
//        return await service.CreateAsync(options, requestOptions);
//    }
//}


using Microsoft.Extensions.Configuration;
using Stripe;

public class PaymentService
{
    private readonly IConfiguration _config;

    public PaymentService(IConfiguration config)
    {
        _config = config;
        StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
    }

    public async Task<PaymentIntent> CreatePaymentIntent(decimal amount, string currency)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100), // cents
            Currency = currency,
            PaymentMethodTypes = new List<string> { "card" }
        };

        var service = new PaymentIntentService();
        return await service.CreateAsync(options);
    }
}