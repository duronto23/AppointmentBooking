using System.Text.Json.Serialization;

namespace AppointmentBooking.Application.DTOs;

public class AppointmentBookingRequest
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
    [JsonPropertyName("products")]
    public string[] Products { get; set; }
    [JsonPropertyName("language")]
    public string Language { get; set; }
    [JsonPropertyName("rating")]
    public string Rating { get; set; }
}