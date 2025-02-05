using AppointmentBooking.Api;
using AppointmentBooking.Application;
using AppointmentBooking.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureCommonServices();
builder.Services.ConfigureApplicationServices();
builder.Services.ConfigureInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.ConfigureMiddleware();

app.Run();