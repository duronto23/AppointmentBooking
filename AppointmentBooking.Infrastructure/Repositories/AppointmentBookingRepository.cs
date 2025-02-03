using Microsoft.EntityFrameworkCore;
using AppointmentBooking.Domain.Entities;
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

    public async Task<List<SlotEntity>> GetSlotEntitiesAsync(DateTime date, string language, string rating, string[] products)
    {
        var bookedSlots = _dbContext.Slots.Where(slot => slot.StartDate.Date == date && slot.Booked == true);

        var notBookedSlots = _dbContext.Slots.Include(s => s.SalesManager).Where(slot =>
            slot.StartDate.Date == date && slot.Booked == false && slot.SalesManager.Languages.Any(l => EF.Functions.ILike(l, language)) &&
            slot.SalesManager.CustomerRatings.Any(r => EF.Functions.ILike(r, rating)) &&
            products.All(pr => slot.SalesManager.Products.Any(p => EF.Functions.ILike(p, pr))));

        var availableSlots = notBookedSlots.Where(slot =>
            bookedSlots.Where(bs => bs.SalesManagerId == slot.SalesManagerId).All(bssm =>
                bssm.EndDate <= slot.StartDate || bssm.StartDate >= slot.EndDate));
        
        return await availableSlots.ToListAsync();
    }
}