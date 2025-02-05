using Microsoft.EntityFrameworkCore;
using AppointmentBooking.Domain.Entities;

namespace AppointmentBooking.Infrastructure.Data;

public class AppointmentBookingDbContext : DbContext
{
    public AppointmentBookingDbContext(DbContextOptions<AppointmentBookingDbContext> options) : base(options)
    {
    }

    public DbSet<SalesManagerEntity> SalesManagers { get; set; }
    public DbSet<SlotEntity> Slots { get; set; }
    public DbSet<AvailableSlots> AvailableSlots { get; set; }
}