using FluentValidation;
using FluentValidation.AspNetCore;
using AppointmentBooking.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using AppointmentBooking.Application.Validation;
using AppointmentBooking.Application.Interfaces;

namespace AppointmentBooking.Application;

public static class ServiceExtension
{
    public static void ConfigureApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICalendarService, CalendarService>();
        services.AddValidatorsFromAssemblyContaining<AppointmentBookingRequestValidator>();
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
    }
}