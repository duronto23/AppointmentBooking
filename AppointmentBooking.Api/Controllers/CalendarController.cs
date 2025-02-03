using AppointmentBooking.Application.DTOs;
using AppointmentBooking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentBooking.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _calendarService;
    
    private readonly ILogger<CalendarController> _logger;

    public CalendarController(ICalendarService calendarService , ILogger<CalendarController> logger)
    {
        _calendarService = calendarService;
        _logger = logger;
    }

    [HttpPost("query")]
    public async Task<IActionResult> QueryAppointments([FromBody] AppointmentBookingRequest bookingRequest)
    {
        _logger.LogInformation("Processing request for available slots on {Date} for a {Rating} customer who speaks {Language} language.", bookingRequest.Date.ToShortDateString(), bookingRequest.Rating, bookingRequest.Language);

        var availableSlots = await _calendarService.GetAvailableSlotsAsync(bookingRequest);

        return availableSlots.Any() ? Ok(availableSlots) : NoContent();
    }
}