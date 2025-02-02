using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using AppointmentBooking.Application.Common;
using AppointmentBooking.Infrastructure.Data;
using AppointmentBooking.Application.Services;
using AppointmentBooking.Application.Interfaces;
using AppointmentBooking.Application.Validation;
using AppointmentBooking.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICalendarService, CalendarService>();
builder.Services.AddScoped<IAppointmentBookingRepository, AppointmentBookingRepository>();

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<AppointmentBookingRequestValidator>();

builder.Services.AddDbContext<AppointmentBookingDbContext>(
    optionBuilder =>
    {
        var connectionString = builder.Configuration.GetConnectionString(Constants.CodingChallengeDbKey);
        optionBuilder.UseNpgsql(connectionString);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();