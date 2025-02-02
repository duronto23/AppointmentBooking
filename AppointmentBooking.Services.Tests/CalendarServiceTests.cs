using Moq;
using NUnit.Framework;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using AppointmentBooking.Domain.Entities;
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
    public async Task RetrievedSlotsSuccessfully()
    {
        //Arrange
        var date = new DateTime(2024, 05, 03, 00, 00, 00, DateTimeKind.Utc);
        var products = new[] { "Heatpumps" };
        var language = "English";
        var rating = "Silver";
        var appointmentBookingRepository = new Mock<IAppointmentBookingRepository>();
        appointmentBookingRepository
            .Setup(r => r.GetSlotEntitiesAsync(It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string[]>())).Returns(Task.FromResult(new List<SlotEntity>
            {
                new() { Id = 3, Booked = false, StartDate = date, EndDate = date.AddHours(1), SalesManagerId = 1 }
            }));

        // Act
        var service = new CalendarService(_mockLogger.Object, appointmentBookingRepository.Object);
        var appointmentBookingRequest = new AppointmentBookingRequest
            { Date = date, Language = language, Products = products, Rating = rating };
        var availableSlots = await service.GetAvailableSlotsAsync(appointmentBookingRequest);

        // Assert
        Assert.That(availableSlots, Is.Not.Null);
        Assert.That(availableSlots, Is.Not.Empty);
        Assert.That(availableSlots.First().AvailableCount, Is.EqualTo(1));
        Assert.That(availableSlots.First().StartDate, Is.EqualTo(date.ToStringUtcFormat()));
    }

    [Test]
    [TestCase("./Resources/TestCaseLangNull.json")]
    [TestCase("./Resources/TestCaseDateNull.json")]
    [TestCase("./Resources/TestCaseRatingNull.json")]
    [TestCase("./Resources/TestCaseProductsEmpty.json")]
    public async Task ValidateRequest(string contentFilePath)
    {
        //Arrange
        var text = await File.ReadAllTextAsync(contentFilePath);
        var bookingRequest = JsonSerializer.Deserialize<AppointmentBookingRequest>(text);
        var date = bookingRequest.Date.GetDateWithUtcKind();
        var products = bookingRequest.Products;
        var language = bookingRequest.Language;
        var rating = bookingRequest.Rating;
        var appointmentBookingRepository = new Mock<IAppointmentBookingRepository>();

        // Act
        var service = new CalendarService(_mockLogger.Object, appointmentBookingRepository.Object);
        var appointmentBookingRequest = new AppointmentBookingRequest
            { Date = date, Language = language, Products = products, Rating = rating };
        var availableSlots = await service.GetAvailableSlotsAsync(appointmentBookingRequest);

        // Assert
        Assert.That(availableSlots, Is.Not.Null);
        Assert.That(availableSlots, Is.Empty);
        _mockLogger.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Invalid request parameters.")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()
        ), Times.Once);
    }
}