using MediatR;
using Newtonsoft.Json;
using TravelOTA.Entitis.Domain;
using TravelOTA.Persistence.Data;

namespace TravelOTA.Application
{
    // public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, int>
    public class CreateBookingCommandHandler
    : IRequestHandler<CreateBookingCommand, CreateBookingResponse>
    {
        private readonly TravelDbContext _context;
        private readonly IAmadeusService _amadeusService;
        private readonly PaymentService _paymentService;
        public CreateBookingCommandHandler(
        TravelDbContext context,
        IAmadeusService amadeusService,
        PaymentService paymentService)
        {
            _context = context;
            _amadeusService = amadeusService;
            _paymentService = paymentService;
        }
        private string GenerateBookingReference()
        {
            return "BK-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }
        // public async Task<object> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        public async Task<CreateBookingResponse> Handle(
     CreateBookingCommand request,
     CancellationToken cancellationToken)
        {
            // 1️⃣ إنشاء Booking فقط
            var booking = new Booking
            {
                UserId = request.UserId,
                BookingReference = GenerateBookingReference(), // مؤقت
                TotalAmount = request.TotalAmount,
                Currency = request.Currency,
                FlightOfferJson = request.FlightOfferJson,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,

                Travelers = request.Travelers.Select(t => new Traveler
                {
                    FirstName = t.FirstName,
                    LastName = t.LastName,
                    DateOfBirth = t.DateOfBirth,
                    Gender = t.Gender,
                    Email = t.Email,
                    PhoneCountryCode = t.PhoneCountryCode,
                    PhoneNumber = t.PhoneNumber,
                    PassportNumber = t.PassportNumber,
                    PassportExpiry = t.PassportExpiry,
                    Nationality = t.Nationality
                }).ToList()
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync(cancellationToken);

            // 2️⃣ إنشاء PaymentIntent
            var paymentIntent = await _paymentService
                .CreatePaymentIntent(booking.TotalAmount, booking.Currency);


            // 3️⃣ إنشاء Payment
            var payment = new Payment
            {
                BookingId = booking.Id,
                Amount = booking.TotalAmount,
                PaymentGateway = "Stripe",
                TransactionId = paymentIntent.Id,
                Status = "Pending"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(cancellationToken);

            // 4️⃣ رجّع للـ Frontend
            //return new
            //{
            //    BookingId = booking.Id,
            //    ClientSecret = paymentIntent.ClientSecret
            //};
            return new CreateBookingResponse
            {
                BookingId = booking.Id,
                ClientSecret = paymentIntent.ClientSecret
            };
        }
        //public async Task<int> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        //{
        //    var pricedFlightOffer =
        //        JsonConvert.DeserializeObject<object>(request.FlightOfferJson);

        //    var firstTraveler = request.Travelers.First();

        //    var orderResponse = await _amadeusService.CreateFlightOrderAsync(
        //        pricedFlightOffer,
        //        new Traveler
        //        {
        //            FirstName = firstTraveler.FirstName,
        //            LastName = firstTraveler.LastName,
        //            DateOfBirth = firstTraveler.DateOfBirth,
        //            Gender = firstTraveler.Gender,
        //            Email = firstTraveler.Email,
        //            PhoneCountryCode = firstTraveler.PhoneCountryCode,
        //            PhoneNumber = firstTraveler.PhoneNumber,
        //            PassportNumber = firstTraveler.PassportNumber,
        //            PassportExpiry = firstTraveler.PassportExpiry,
        //            Nationality = firstTraveler.Nationality
        //        });
        //    Console.WriteLine(orderResponse);
        //    dynamic json = JsonConvert.DeserializeObject(orderResponse);

        //    // string pnr = json?.data?.associatedRecords?[0]?.reference;
        //    string pnr = json?.data?.associatedRecords?[0]?.reference
        //   ?? json?.data?.id;
        //    if (string.IsNullOrEmpty(pnr))
        //    {
        //        pnr = GenerateBookingReference();
        //    }
        //    var booking = new Booking
        //    {
        //        UserId = request.UserId,
        //        BookingReference = pnr,
        //        TotalAmount = request.TotalAmount,
        //        Currency = request.Currency,
        //        FlightOfferJson = request.FlightOfferJson,
        //        Status = "Confirmed",
        //        CreatedAt = DateTime.UtcNow,

        //        Travelers = request.Travelers.Select(t => new Traveler
        //        {
        //            FirstName = t.FirstName,
        //            LastName = t.LastName,
        //            DateOfBirth = t.DateOfBirth,
        //            Gender = t.Gender,
        //            Email = t.Email,
        //            PhoneCountryCode = t.PhoneCountryCode,
        //            PhoneNumber = t.PhoneNumber,
        //            PassportNumber = t.PassportNumber,
        //            PassportExpiry = t.PassportExpiry,
        //            Nationality = t.Nationality
        //        }).ToList()
        //    };

        //    _context.Bookings.Add(booking);

        //    await _context.SaveChangesAsync(cancellationToken);

        //    return booking.Id;
        //}

    }
}