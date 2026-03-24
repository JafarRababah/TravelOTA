
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using TravelOTA.Application;
using TravelOTA.Persistence.Data;

namespace TravelOTA.API
    {
        [ApiController]
        [Route("api/[controller]")]
        public class BookingsController : ControllerBase
        {
            private readonly IMediator _mediator;
            private readonly TravelDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PaymentService _paymentService;
        public BookingsController(IMediator mediator, TravelDbContext context, IConfiguration configuration)
        {
            _mediator = mediator;
            _context = context;
            _configuration = configuration;
        }

        //    [HttpPost]
        //    [Authorize] // يتطلب تسجيل دخول
        //    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command)
        //    {
        //    // 1️⃣ استخراج UserId من JWT
        //    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        //    command.UserId = userId;

        //    var id = await _mediator.Send(command);

        //    return Ok(new { BookingId = id });
        //}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("UserId claim missing in token.");

            command.UserId = int.Parse(userIdClaim);

            //var id = await _mediator.Send(command);

            //return Ok(new { BookingId = id });
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Travelers)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();

            return Ok(booking);
        }
        [HttpGet("my-bookings")]
        [Authorize]
        public async Task<IActionResult> GetMyBookings()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Travelers)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return Ok(bookings);
        }
        [HttpPut("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            if (booking.Status == "Cancelled")
                return BadRequest(new { message = "Already cancelled" });

            booking.Status = "Cancelled";

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cancelled successfully" });
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            Request.EnableBuffering();

            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();
            Request.Body.Position = 0;

            Console.WriteLine("JSON: " + json);

            var signature = Request.Headers["Stripe-Signature"].FirstOrDefault();

            Console.WriteLine("Signature: " + signature);

            if (string.IsNullOrEmpty(json))
                return BadRequest("Empty JSON");

            if (string.IsNullOrEmpty(signature))
                return BadRequest("Missing Signature");

            var stripeEvent = EventUtility.ConstructEvent(
                json,
                signature,
                "whsec_7d3ffb18d7edcc9a5e97adc8a186e66ded5f55fb8e21d94b621d45e5a1c20195",
                throwOnApiVersionMismatch: false
            );

            Console.WriteLine("Event Type: " + stripeEvent.Type);

            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var intent = stripeEvent.Data.Object as PaymentIntent;

                var payment = await _context.Payments
                    .Include(p => p.Booking)
                    .FirstOrDefaultAsync(p => p.TransactionId == intent.Id);

                if (payment != null)
                {
                    payment.Status = "Succeeded";

                    if (payment.Booking != null)
                        payment.Booking.Status = "Confirmed";

                    await _context.SaveChangesAsync();
                }
            }

            return Ok();
        }
    }
    }



