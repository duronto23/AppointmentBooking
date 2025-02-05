namespace AppointmentBooking.Api;

public static class ServiceExtension
{
    public static void ConfigureCommonServices(this IServiceCollection services)
    {
        services.AddControllers();
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
}