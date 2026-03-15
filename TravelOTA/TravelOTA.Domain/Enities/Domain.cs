using System;

namespace TravelOTA.Entitis.Domain
{
   
    public class FlightSearch
    {
        public Guid Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime DepartureDate { get; set; }
        public int Adults { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Booking
    {
        public int Id { get; set; }

        public string BookingReference { get; set; }

        public int UserId { get; set; }

        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }

        public string Status { get; set; }

        public string FlightOfferJson { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Traveler> Travelers { get; set; } = new();
        public Payment? Payment { get; set; }
    }

    public class Payment
    {
        public int Id { get; set; }

        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        public decimal Amount { get; set; }

        public string PaymentGateway { get; set; }

        public string TransactionId { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    public class Traveler
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public DateTime DateOfBirth { get; set; }

        // MALE / FEMALE
        public string Gender { get; set; } = "MALE";

        public string Email { get; set; } = "";

        public string PhoneCountryCode { get; set; } = "962";
        public string PhoneNumber { get; set; } = "";

        public string PassportNumber { get; set; } = "";
        public string PassportExpiry { get; set; } = ""; // yyyy-MM-dd
        public string Nationality { get; set; } = "JO";
    }
    public enum BookingStatus
    {
        Pending = 1,
        Confirmed = 2,
        Cancelled = 3,
        Failed = 4
    }

    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string FullName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
