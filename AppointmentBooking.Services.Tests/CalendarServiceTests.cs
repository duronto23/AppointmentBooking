using Moq;
using NUnit.Framework;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using AppointmentBooking.Application.DTOs;
using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.Services;
using AppointmentBooking.Application.Interfaces;

namespace AppointmentBooking.Services.Test;

[TestFixture]
public class CalendarServiceTests
{
    private Mock<ILogger<CalendarService>> _mockLogger;

    [SetUp]
    public void SetUp()
    {
        _mockLogger = new Mock<ILogger<CalendarService>>();
    }

    [Test]
    [TestCase("./Resources/TestCase1.json")]
    [TestCase("./Resources/TestCase2.json")]
    [TestCase("./Resources/TestCase3.json")]
    [TestCase("./Resources/TestCase4.json")]
    public async Task ValidateRequest(string contentFilePath)
    {
        //Arrange
        var text = await File.ReadAllTextAsync(contentFilePath);
        var bookingRequest = JsonSerializer.Deserialize<AppointmentBookingRequest>(text);
        var date = bookingRequest.Date.GetDateWithUtcKind();
        var products = bookingRequest.Products.Select(p => p.ToLowerInvariant());
        var language = bookingRequest.Language.ToLowerInvariant();
        var rating = bookingRequest.Rating.ToLowerInvariant();
        var appointmentBookingRepository = new Mock<IAppointmentBookingRepository>();
        appointmentBookingRepository
            .Setup(r => r.GetSlotEntitiesAsync(It.Is<DateTime>(d => d.Date == date.Date), language, rating,
                It.Is<string[]>(arr => arr.SequenceEqual(products)))).Returns(Task.FromResult(new List<DateTime> { date }));

        // Act
        var service = new CalendarService(_mockLogger.Object, appointmentBookingRepository.Object);
        var availableSlots = await service.GetAvailableSlotsAsync(bookingRequest);

        // Assert
        Assert.That(availableSlots, Is.Not.Null);
        Assert.That(availableSlots, Is.Not.Empty);
        Assert.That(availableSlots.First().AvailableCount, Is.EqualTo(1));
        Assert.That(availableSlots.First().StartDate, Is.EqualTo(date.ToStringUtcFormat()));
        
        _mockLogger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains($"Total 1 slots found for {language} language, {rating} rating and products: {string.Join(",", products)}.")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()
        ), Times.Once);
    }
}