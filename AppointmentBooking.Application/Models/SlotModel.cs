using System.Text.Json.Serialization;

namespace AppointmentBooking.Application.Models;

public class SlotModel
{
    [JsonPropertyName("available_count")]
    public int AvailableCount { get; set; }
    [JsonPropertyName("start_date")]
    public required string StartDate { get; set; }
}