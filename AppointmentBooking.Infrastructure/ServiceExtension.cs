using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AppointmentBooking.Application.Common;
using AppointmentBooking.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using AppointmentBooking.Application.Interfaces;
using AppointmentBooking.Infrastructure.Repositories;

namespace AppointmentBooking.Infrastructure;

public static class ServiceExtension
{
    public static void ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAppointmentBookingRepository, AppointmentBookingRepository>();
        
        services.AddDbContext<AppointmentBookingDbContext>(
            optionBuilder =>
            {
                var connectionString = configuration.GetConnectionString(Constants.CodingChallengeDbKey);
                optionBuilder.UseNpgsql(connectionString);
            });
    }
}