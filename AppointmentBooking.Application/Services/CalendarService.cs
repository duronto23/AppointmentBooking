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
        if (request?.Date == DateTime.MinValue || request?.Language == null || request?.Rating == null || request?.Products == null)
        {
            _logger.LogError("Invalid request parameters.");
            return Array.Empty<SlotModel>();
        }
        
        var date = request.Date.GetDateWithUtcKind();
        var language = request.Language.ToLowerInvariant();
        var rating = request.Rating.ToLowerInvariant();
        var products = request.Products.Select(p => p.ToLowerInvariant()).ToArray();

        var slotsByManager = await _appointmentBookingRepository.GetSlotEntitiesAsync(date, language, rating, products);
        
        return slotsByManager.GroupBy(slot => slot.StartDate).Select(group => new SlotModel
            { StartDate = group.Key.ToStringUtcFormat(), AvailableCount = group.Count() });
    }
}