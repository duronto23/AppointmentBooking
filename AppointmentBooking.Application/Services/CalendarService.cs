using Microsoft.Extensions.Logging;
using AppointmentBooking.Application.DTOs;
using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.Models;
using AppointmentBooking.Application.Interfaces;

namespace AppointmentBooking.Application.Services;

public class CalendarService : ICalendarService
{
    private readonly IAppointmentBookingRepository _appointmentBookingRepository;
    private readonly ILogger<CalendarService> _logger;

    public CalendarService(ILogger<CalendarService> logger, IAppointmentBookingRepository appointmentBookingRepository)
    {
        _logger = logger;
        _appointmentBookingRepository = appointmentBookingRepository;
    }
    
    public async Task<IEnumerable<SlotModel>> GetAvailableSlotsAsync(AppointmentBookingRequest request)
    {
        if (request?.Date == DateTime.MinValue || request?.Language == null || request?.Rating == null || request?.Products == null)
        {
            _logger.LogError("Invalid request parameters.");
            return Array.Empty<SlotModel>();
        }
        
        var date = request.Date.GetDateWithUtcKind();
        var language = request.Language.ToLowerInvariant();
        var rating = request.Rating.ToLowerInvariant();
        var products = request.Products.Select(p => p.ToLowerInvariant()).ToArray();

        var slotsByManager = await _appointmentBookingRepository.GetSlotEntitiesAsync(date, language, rating, products);
        
        return slotsByManager.GroupBy(slot => slot.StartDate).Select(group => new SlotModel
            { StartDate = group.Key.ToStringUtcFormat(), AvailableCount = group.Count() });
    }

    // private bool IsValidParams(DateTime date, string language, string rating, string[] products)
    // {
    //     return date.Year >= 1900 && date.Year <= 2100 && !string.IsNullOrEmpty(language) &&
    //            !string.IsNullOrEmpty(rating) && products?.Length > 0;
    // }
    //
    // private (DateTime date, string language, string rating, string[] products) GetRequestParams(AppointmentBookingRequest bookingRequest)
    // {
    //     if (bookingRequest?.Date == DateTime.MinValue || bookingRequest?.Language == null ||
    //         bookingRequest?.Rating == null || bookingRequest?.Products == null)
    //     {
    //         _logger.LogError("Invalid request parameters.");
    //         return (DateTime.MinValue, string.Empty, string.Empty, []);
    //     }
    //
    //     var date = bookingRequest.Date.GetDateWithUtcKind();
    //     var language = bookingRequest.Language.ToLowerInvariant();
    //     var rating = bookingRequest.Rating.ToLowerInvariant();
    //     var products = bookingRequest.Products.Select(p => p.ToLowerInvariant()).ToArray();
    //     return (date, language, rating, products);
    // }
    //
    // public async Task<IEnumerable<SlotModel>> GetAvailableSlotsAsync1(AppointmentBookingRequest bookingRequest)
    // {
    //     var (date, language, rating, products) = GetRequestParams(bookingRequest);
    //     if(!IsValidParams(date, language, rating, products))
    //         return Array.Empty<SlotModel>();
    //
    //     var slotsByManager = await _appointmentBookingRepository.GetSlotEntitiesAsync(date, language, rating, products);
    //     
    //     var availableSlots = await GetAvailableSlots(slotsByManager);
    //     
    //     _logger.LogInformation($"Retrieved {slotsByManager.Count()} available slots");
    //     
    //     return availableSlots;
    // }

    // private async Task<IEnumerable<SlotModel>> GetAvailableSlots(List<SlotEntity> slots)
    // {
    //     if (slots?.Count == 0)
    //     {
    //         _logger.Log(LogLevel.Warning, "No available slots found.");
    //         return [];
    //     }
    //     
    //     var slotsByManager = slots.GroupBy(slot => slot.SalesManagerId);
    //     var availableSlots = new Dictionary<string, int>();
    //     await Task.WhenAll(slotsByManager.AsEnumerable().Select(async slot =>
    //     {
    //         await Task.WhenAll(GetBookableSlots(slot).Select(bookableSlot =>
    //         {
    //             var dateString = bookableSlot.StartDate.ToStringUtcFormat();
    //             lock (availableSlots)
    //             {
    //                 if (!availableSlots.TryAdd(dateString, 1))
    //                     availableSlots[dateString]++;
    //             }
    //             return Task.CompletedTask;
    //         }));
    //     }));
    //     return availableSlots.Select(availableSlot => new SlotModel{AvailableCount = availableSlot.Value, StartDate = availableSlot.Key});
    // }
    
    // private static IEnumerable<SlotEntity> GetBookableSlots(IGrouping<long, SlotEntity> slotsGroup)
    // {
    //     var slotEntities = slotsGroup.OrderBy(slot => slot.StartDate);
    //     return slotEntities.Where((slot, index) =>
    //         !slot.Booked && (index <= 0 || !OverlapWithPreviousBookedSlot(slotEntities.ElementAt(index - 1), slot)) &&
    //         (index == slotEntities.Count() - 1 || !OverlapWithNextBookedSlot(slotEntities.ElementAt(index + 1), slot)));
    // }
    
    // private static bool OverlapWithPreviousBookedSlot(SlotEntity previousSlotEntity, SlotEntity slotEntity)
    // {
    //     return previousSlotEntity.Booked && previousSlotEntity.EndDate > slotEntity.StartDate;
    // }
    //
    // private static bool OverlapWithNextBookedSlot(SlotEntity nextSlotEntity, SlotEntity slotEntity)
    // {
    //     return nextSlotEntity.Booked && nextSlotEntity.StartDate < slotEntity.EndDate;
    // }
    
    /*
    public async Task<IEnumerable<SlotModel>> GetAvailableSlotsAsync2(AppointmentBookingRequest bookingRequest)
    {
        var date = new DateTime(bookingRequest.Date.Year, bookingRequest.Date.Month, bookingRequest.Date.Day, 0, 0, 0, DateTimeKind.Utc);
        var language = bookingRequest.Language.ToLowerInvariant();
        var rating = bookingRequest.Rating.ToLowerInvariant();
        var products = bookingRequest.Products.Select(p => p.ToLowerInvariant()).ToArray();
        
        var managers = _dbContext.SalesManagers.Where(m => m.Languages.Any(lan => EF.Functions.ILike(lan, language)) &&
                                                                 m.CustomerRatings.Any(rat => EF.Functions.ILike(rat, rating)) &&
                                                                 products.All(pr => m.Products.Any(pro => EF.Functions.ILike(pro, pr))))
            .Select(manager => manager.Id);

        var slotsGroups = _dbContext.Slots.Where(s => s.StartDate.Date == date && managers.Contains(s.SalesManagerId))
            .GroupBy(s => s.SalesManagerId);

        var availableSlots = new Dictionary<string, int>();

        foreach (var slotsGroup in slotsGroups)
        {
            var bookableSlots = GetBookableSlots(slotsGroup);
            
            

            foreach (var stringDate in bookableSlots.Select(bookableSlot => bookableSlot.StartDate.ToString(SlotDateFormat)).Where(stringDate => !availableSlots.TryAdd(stringDate, 1)))
            {
                availableSlots[stringDate]++;
            }
        }

        return availableSlots.Select(sl => new SlotModel{AvailableCount = sl.Value, StartDate = sl.Key});
    }
    
    public async Task<IEnumerable<SlotModel>> GetAvailableSlotsAsync1(AppointmentBookingRequest bookingRequest)
    {
        var managersFromDb = await _dbContext.SalesManagers.ToListAsync();
        
        var managers = managersFromDb.Where(m => 
            m.Languages.Any(l => l.ToString() == bookingRequest.Language) && 
            m.CustomerRatings.Any(r => r.ToString() == bookingRequest.Rating) &&
            bookingRequest.Products.All(p => m.Products.Any(mp => mp.ToString() == p.ToString())));

        var slotsGroups = _dbContext.Slots.Where(s => managers.Select(m => m.Id).ToList().Contains(s.SalesManagerId))
            .GroupBy(s => s.SalesManagerId);

        var availableSlots = new Dictionary<string, int>();

        foreach (var slotsGroup in slotsGroups)
        {
            var bookableSlots = GetBookableSlots(slotsGroup);

            foreach (var stringDate in bookableSlots.Select(bookableSlot => bookableSlot.StartDate.ToString(SlotDateFormat)).Where(stringDate => !availableSlots.TryAdd(stringDate, 1)))
            {
                availableSlots[stringDate]++;
            }
        }

        return availableSlots.Select(sl => new SlotModel{AvailableCount = sl.Value, StartDate = sl.Key});
    }
    */
}