using Microsoft.Extensions.Logging;
using AppointmentBooking.Application.DTOs;
using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.Models;
using AppointmentBooking.Application.Interfaces;

namespace AppointmentBooking.Application.Services;

public class CalendarService : ICalendarService
{
    private readonly IAppointmentBookingRepository _appointmentBookingRepository;
    private readonly ILogger<CalendarService> _logger;

    public CalendarService(ILogger<CalendarService> logger, IAppointmentBookingRepository appointmentBookingRepository)
    {
        _logger = logger;
        _appointmentBookingRepository = appointmentBookingRepository;
    }
    
    public async Task<IEnumerable<SlotModel>> GetAvailableSlotsAsync(AppointmentBookingRequest request)
    {
        var date = request.Date.GetDateWithUtcKind();
        var language = request.Language.ToLowerInvariant();
        var rating = request.Rating.ToLowerInvariant();
        var products = request.Products.Select(p => p.ToLowerInvariant()).ToArray();
        
        var availableSlots = await _appointmentBookingRepository.GetSlotEntitiesAsync(date, language, rating, products);

        _logger.LogInformation($"Total {availableSlots.Count} slots found for {language} language, {rating} rating and products: {string.Join(",", products)}.");
        
        return availableSlots.GroupBy(slot => slot).Select(group => new SlotModel
            { StartDate = group.Key.ToStringUtcFormat(), AvailableCount = group.Count() });
    }
}