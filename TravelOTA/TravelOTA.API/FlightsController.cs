using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using TravelOTA.Application;
using TravelOTA.Entitis.Domain;

namespace TravelOTA.API { 
    [ApiController][Route("api/[controller]")]
    public class FlightsController : ControllerBase 
    {
        private readonly IAmadeusService _amadeusService; public FlightsController(IAmadeusService amadeusService)
        { 
            _amadeusService = amadeusService; 
        }

        //[HttpPost("search")]
        //public async Task<IActionResult> Search([FromBody] FlightSearchRequest request)
        //{
        //    var jsonResult = await _amadeusService.SearchFlightsAsync(request);

        //    var response = JsonConvert.DeserializeObject<AmadeusFlightOfferResponse>(jsonResult);

        //    if (response == null || response.Data == null || !response.Data.Any())
        //        return NotFound("No flights found.");

        //    var flights = response.Data.Select(o => new
        //    {
        //        rawFlightOffer = o
        //    });

        //    return Ok(flights);

        //}

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] FlightSearchRequest request)
        {
            var jsonResult = await _amadeusService.SearchFlightsAsync(request);

            var root = JObject.Parse(jsonResult);

            var offers = root["data"];

            if (offers == null || !offers.Any())
                return NotFound("No flights returned.");

            var firstOffer = offers[0];

            var result = new JObject
            {
                ["rawFlightOffer"] = firstOffer
            };


            return Content(result.ToString(), "application/json");
        }
        [HttpPost("pricing")]
        public async Task<IActionResult> Pricing([FromBody] PricingRequest request)
        {
            if (request.RawFlightOffer.ValueKind == JsonValueKind.Undefined)
                return BadRequest("RawFlightOffer is required");

            var result = await _amadeusService.ConfirmPricingAsync(request.RawFlightOffer);

            return Content(result, "application/json");
        }
        //[HttpPost("book")]
        //public async Task<IActionResult> BookFlight([FromBody] BookingRequestDto request)
        //{
        //    var pricingResult = await _amadeusService.ConfirmPricingAsync(request.FlightOffer);
        //    dynamic pricingJson = JsonConvert.DeserializeObject(pricingResult);
        //    var pricedOffer = pricingJson.data.flightOffers[0];

        //    var orderResult = await _amadeusService.CreateFlightOrderAsync(pricedOffer, request.Traveler);

        //    dynamic orderJson = JsonConvert.DeserializeObject(orderResult);

        //    var orderId = orderJson.data.id;
        //    var pnr = orderJson.data.associatedRecords[0].reference;
        //    var total = decimal.Parse((string)orderJson.data.flightOffers[0].price.total);
        //    var currency = (string)orderJson.data.flightOffers[0].price.currency;

        //    var booking = new Booking
        //    {
        //        AmadeusOrderId = orderId,
        //        PNR = pnr,
        //        FirstName = request.Traveler.FirstName,
        //        LastName = request.Traveler.LastName,
        //        Email = request.Traveler.Email,
        //        TotalPrice = total,
        //        Currency = currency
        //    };

        //    _context.Bookings.Add(booking);
        //    await _context.SaveChangesAsync();

        //    return Ok(new
        //    {
        //        bookingId = booking.Id,
        //        pnr = booking.PNR,
        //        total = booking.TotalPrice,
        //        currency = booking.Currency
        //    });
        //}


    }

}


