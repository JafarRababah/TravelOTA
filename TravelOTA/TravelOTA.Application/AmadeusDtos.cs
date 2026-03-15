using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelOTA.Application
{
    
        public class AmadeusFlightOfferResponse
        {
            public List<AmadeusFlightOffer> Data { get; set; } = new();
        }

        public class AmadeusFlightOffer
        {
            public string Id { get; set; } = "";
            public AmadeusPrice Price { get; set; } = new();
            public List<AmadeusItinerary> Itineraries { get; set; } = new();
        }

        public class AmadeusPrice
        {
            public string Currency { get; set; } = "";
            public string Total { get; set; } = "";
        }

        public class AmadeusItinerary
        {
            public List<AmadeusSegment> Segments { get; set; } = new();
        }

        public class AmadeusSegment
        {
            public AmadeusFlightPoint Departure { get; set; } = new();
            public AmadeusFlightPoint Arrival { get; set; } = new();
            public string CarrierCode { get; set; } = "";
            public string Number { get; set; } = "";
        }

        public class AmadeusFlightPoint
        {
            public string IataCode { get; set; } = "";
            public DateTime At { get; set; }
        }

    
}
