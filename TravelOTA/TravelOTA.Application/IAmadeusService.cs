using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TravelOTA.Entitis.Domain;
namespace TravelOTA.Application { 
    public interface IAmadeusService {
        Task<string> SearchFlightsAsync(FlightSearchRequest request);

        Task<string> ConfirmPricingAsync(JsonElement flightOffer);

        Task<string> CreateFlightOrderAsync(object pricedFlightOffer, Traveler traveler);
    }
    public class AmadeusService : IAmadeusService {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public AmadeusService(HttpClient httpClient, IConfiguration configuration) {
            _httpClient = httpClient; _configuration = configuration; }
        private async Task<string> GetAccessTokenAsync()
        { var clientId = _configuration["Amadeus:ClientId"]; var clientSecret = _configuration["Amadeus:ClientSecret"]; 
            var request = new HttpRequestMessage(HttpMethod.Post, "https://test.api.amadeus.com/v1/security/oauth2/token"); 
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "grant_type", "client_credentials" }, { "client_id", clientId }, { "client_secret", clientSecret } });
            var response = await _httpClient.SendAsync(request); 
            var content = await response.Content.ReadAsStringAsync();
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(content); return json.access_token; } 
        public async Task<string> SearchFlightsAsync(FlightSearchRequest request) { 
            var token = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var url = $"https://test.api.amadeus.com/v2/shopping/flight-offers?" + $"originLocationCode={request.Origin}" + $"&destinationLocationCode={request.Destination}" + $"&departureDate={request.DepartureDate:yyyy-MM-dd}" + $"&adults={request.Adults}"; var response = await _httpClient.GetAsync(url); return await response.Content.ReadAsStringAsync();
        }
 
        public async Task<string> ConfirmPricingAsync(JsonElement flightOffer)
        {
            var token = await GetAccessTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                data = new
                {
                    type = "flight-offers-pricing",
                    flightOffers = new object[]
                    {
                System.Text.Json.JsonSerializer.Deserialize<object>(flightOffer.GetRawText())
                    }
                }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(request);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                "https://test.api.amadeus.com/v1/shopping/flight-offers/pricing",
                content
            );

            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> CreateFlightOrderAsync(object pricedFlightOffer, Traveler traveler)
        {
            var token = await GetAccessTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var travelerObject = new
            {
                id = "1",
                dateOfBirth = traveler.DateOfBirth.ToString("yyyy-MM-dd"),
                name = new
                {
                    firstName = traveler.FirstName.ToUpper(),
                    lastName = traveler.LastName.ToUpper()
                },
                gender = traveler.Gender,
                contact = new
                {
                    emailAddress = traveler.Email,
                    phones = new[]
                    {
                new {
                    deviceType = "MOBILE",
                    countryCallingCode = traveler.PhoneCountryCode,
                    number = traveler.PhoneNumber
                }
            }
                },
                documents = new[]
                {
            new {
                documentType = "PASSPORT",
                number = traveler.PassportNumber,
                expiryDate = traveler.PassportExpiry,
                issuanceCountry = traveler.Nationality,
                nationality = traveler.Nationality
            }
        }
            };

            var requestBody = new
            {
                data = new
                {
                    type = "flight-order",
                    flightOffers = new[] { pricedFlightOffer },
                    travelers = new[] { travelerObject }
                }
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "https://test.api.amadeus.com/v1/booking/flight-orders",
                content);

            return await response.Content.ReadAsStringAsync();
        }


    }

}

