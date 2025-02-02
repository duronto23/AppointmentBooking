using AppointmentBooking.Domain.Entities;

namespace AppointmentBooking.Application.Interfaces;

public interface IAppointmentBookingRepository
{
    Task<List<SlotEntity>> GetSlotEntitiesAsync(DateTime date, string language, string rating, string[] products);
}