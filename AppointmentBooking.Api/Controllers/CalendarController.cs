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
        return Ok(await _calendarService.GetAvailableSlotsAsync(bookingRequest));
    }
}