using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc.Testing;
using AppointmentBooking.Application.DTOs;
using AppointmentBooking.Application.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AppointmentBooking.Api.Tests;

public class CalendarControllerTests
{
    [Test]
    [TestCase("./Resources/TestCase1.json", TestName = "Monday 2024-05-03, Solar Panels and Heatpumps, German and Gold customer. Only Seller 2 is selectable.")]
    [TestCase("./Resources/TestCase2.json", TestName = "Monday 2024-05-03, Heatpumps, English and Silver customer. Both Seller 2 and Seller 3 are selectable.")]
    [TestCase("./Resources/TestCase3.json", TestName = "Monday 2024-05-03, SolarPanels, German and Bronze customer. All Seller 1 and 2 are selectable, but Seller 1 does not have available slots.")]
    [TestCase("./Resources/TestCase4.json", TestName = "Tuesday 2024-05-04, Solar Panels and Heatpumps, German and Gold customer. Only Seller 2 is selectable, but it is fully booked.")]
    [TestCase("./Resources/TestCase5.json", TestName = "Tuesday 2024-05-04, Heatpumps, English and Silver customer. Both Seller 2 and Seller 3 are selectable, but Seller 2 is fully booked.")]
    [TestCase("./Resources/TestCase6.json", TestName = "Monday 2024-05-03, SolarPanels, German and Bronze customer. Seller 1 and 2 are selectable, but Seller 2 is fully booked.")]
    [TestCase("./Resources/TestCase7.json", TestName = "Tuesday 2024-05-04, Heatpumps, German and Gold customer. One available slot.")]
    [TestCase("./Resources/TestCase8.json", TestName = "Tuesday 2024-05-04, Heatpumps, German and Silver customer. One available slot.")]
    [TestCase("./Resources/TestCase9.json", TestName = "Tuesday 2024-05-04, SolarPanels, German and Gold customer. No available slot.")]
    public async Task QueryAppointments_Response_ShouldMatch(string testCaseFilePath)
    {
        // Arange
        var (request, expectedResponse) = JsonSerializer.Deserialize<Tuple<AppointmentBookingRequest, List<SlotModel>>>(await File.ReadAllTextAsync(testCaseFilePath));
        var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateDefaultClient();
        var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        // Act
        var response = await client.PostAsync("calendar/query", requestContent);
        
        // Assert
        Assert.That(response.StatusCode.ToString(), Is.EqualTo("OK"));
        Assert.That(response.Content, Is.Not.Null);
        var responseContent = await response.Content.ReadAsStringAsync();
        var slotModels = JsonSerializer.Deserialize<List<SlotModel>>(responseContent);
        Assert.That(slotModels.Count, Is.EqualTo(expectedResponse.Count));
        for (var i = 0; i < slotModels.Count; i++)
        {
            Assert.That(slotModels[i].StartDate, Is.EqualTo(expectedResponse[i].StartDate));
            Assert.That(slotModels[i].AvailableCount, Is.EqualTo(expectedResponse[i].AvailableCount));
        }
    }
}