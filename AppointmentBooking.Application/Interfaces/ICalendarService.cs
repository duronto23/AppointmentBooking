using AppointmentBooking.Application.DTOs;
using AppointmentBooking.Application.Models;

namespace AppointmentBooking.Application.Interfaces;

public interface ICalendarService
{
    Task<IEnumerable<SlotModel>> GetAvailableSlotsAsync(AppointmentBookingRequest request);
}