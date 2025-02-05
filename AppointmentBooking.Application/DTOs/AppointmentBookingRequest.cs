using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AppointmentBooking.Application.DTOs;

public class AppointmentBookingRequest
{
    [Required]
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
    [Required]
    [JsonPropertyName("products")]
    public string[] Products { get; set; }
    [Required]
    [JsonPropertyName("language")]
    public string Language { get; set; }
    [Required]
    [JsonPropertyName("rating")]
    public string Rating { get; set; }
}