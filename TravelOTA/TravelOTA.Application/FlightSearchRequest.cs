using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TravelOTA.Application
{
    public class FlightSearchRequest
    {
        public string Origin { get; set; } = null;
        public string Destination { get; set; } = null;
        public DateTime DepartureDate { get; set; }
        public int Adults { get; set; } = 1;
    }
    public class FlightOffer
    {
        public string Airline { get; set; } = null!;
        public string Origin { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public DateTime Departure { get; set; }
        public DateTime Arrival { get; set; }
        public decimal Price { get; set; }
        public dynamic RawFlightOffer { get; set; } = null;
       
    }
    public class PricingRequest
    {

        public JsonElement RawFlightOffer { get; set; }
    }
}