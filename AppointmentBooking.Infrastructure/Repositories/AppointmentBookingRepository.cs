using Microsoft.EntityFrameworkCore;
using AppointmentBooking.Infrastructure.Data;
using AppointmentBooking.Application.Interfaces;

namespace AppointmentBooking.Infrastructure.Repositories;

public class AppointmentBookingRepository : IAppointmentBookingRepository
{
    private readonly AppointmentBookingDbContext _dbContext;

    public AppointmentBookingRepository(AppointmentBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<DateTime>> GetSlotEntitiesAsync(DateTime date, string language, string rating, string[] products)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);
        
        var availableSlots = _dbContext.AvailableSlots.Where(slot =>
            slot.StartDate >= startOfDay && slot.StartDate < endOfDay &&
            slot.Languages.Any(l => language.Equals(l.ToLower())) &&
            slot.CustomerRatings.Any(r => rating.Equals(r.ToLower())) &&
            products.All(pr => slot.Products.Any(p => pr.Equals(p.ToLower())))).Select(s => s.StartDate);
            
        return await availableSlots.ToListAsync();
    }
}