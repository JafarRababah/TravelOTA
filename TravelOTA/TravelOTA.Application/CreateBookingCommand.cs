using MediatR;
using System;
using System.Collections.Generic;

namespace TravelOTA.Application
{
    public class CreateBookingCommand : IRequest<int>
    {
        public int UserId { get; set; }

        public string FlightNumber { get; set; }

        public DateTime TravelDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Currency { get; set; }

        public string FlightOfferJson { get; set; }

        public List<TravelerDto> Travelers { get; set; } = new();
    }
}