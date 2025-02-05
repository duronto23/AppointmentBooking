namespace AppointmentBooking.Application.Interfaces;

public interface IAppointmentBookingRepository
{
    Task<List<DateTime>> GetSlotEntitiesAsync(DateTime date, string language, string rating, string[] products);
}