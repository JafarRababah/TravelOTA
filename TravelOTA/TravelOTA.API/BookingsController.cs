
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelOTA.Application;

namespace TravelOTA.API
    {
        [ApiController]
        [Route("api/[controller]")]
        public class BookingsController : ControllerBase
        {
            private readonly IMediator _mediator;

            public BookingsController(IMediator mediator)
            {
                _mediator = mediator;
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

            var id = await _mediator.Send(command);

            return Ok(new { BookingId = id });
        }
    }
    }



