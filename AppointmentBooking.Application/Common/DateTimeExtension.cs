namespace AppointmentBooking.Application.Common;

public static class DateTimeExtension
{
    public static DateTime GetDateWithUtcKind(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc);
    }

    public static string ToStringUtcFormat(this DateTime dateTime)
    {
        return dateTime.ToString(Constants.SlotDateFormat);
    }
}