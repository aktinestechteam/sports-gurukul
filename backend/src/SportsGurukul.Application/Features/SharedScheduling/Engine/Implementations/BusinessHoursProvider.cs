using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public class BusinessHoursProvider : IBusinessHoursProvider
{
    private readonly ILogger<BusinessHoursProvider> _logger;

    public BusinessHoursProvider(ILogger<BusinessHoursProvider> logger) => _logger = logger;

    public Task<IReadOnlyList<BusinessHours>> GetBusinessHoursAsync(Guid resourceId, string resourceType, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Returning default business hours for {ResourceType}:{ResourceId}", resourceType, resourceId);
        IReadOnlyList<BusinessHours> hours =
        [
            new BusinessHours { DayOfWeek = DayOfWeek.Monday, OpenTime = new TimeSpan(6, 0, 0), CloseTime = new TimeSpan(22, 0, 0) },
            new BusinessHours { DayOfWeek = DayOfWeek.Tuesday, OpenTime = new TimeSpan(6, 0, 0), CloseTime = new TimeSpan(22, 0, 0) },
            new BusinessHours { DayOfWeek = DayOfWeek.Wednesday, OpenTime = new TimeSpan(6, 0, 0), CloseTime = new TimeSpan(22, 0, 0) },
            new BusinessHours { DayOfWeek = DayOfWeek.Thursday, OpenTime = new TimeSpan(6, 0, 0), CloseTime = new TimeSpan(22, 0, 0) },
            new BusinessHours { DayOfWeek = DayOfWeek.Friday, OpenTime = new TimeSpan(6, 0, 0), CloseTime = new TimeSpan(22, 0, 0) },
            new BusinessHours { DayOfWeek = DayOfWeek.Saturday, OpenTime = new TimeSpan(7, 0, 0), CloseTime = new TimeSpan(20, 0, 0) },
            new BusinessHours { DayOfWeek = DayOfWeek.Sunday, OpenTime = new TimeSpan(8, 0, 0), CloseTime = new TimeSpan(18, 0, 0) },
        ];
        return Task.FromResult(hours);
    }

    public async Task<bool> IsWithinBusinessHoursAsync(Guid resourceId, string resourceType, TimeSlot slot, CancellationToken cancellationToken = default)
    {
        var hours = await GetBusinessHoursAsync(resourceId, resourceType, cancellationToken);
        var dayHours = hours.FirstOrDefault(h => h.DayOfWeek == slot.Date.DayOfWeek);
        return dayHours?.ContainsSlot(slot) ?? false;
    }

    public async Task<IReadOnlyList<TimeSlot>> GetBusinessHourSlotsAsync(Guid resourceId, string resourceType, DateTime date, TimeSpan slotDuration, CancellationToken cancellationToken = default)
    {
        var hours = await GetBusinessHoursAsync(resourceId, resourceType, cancellationToken);
        var dayHours = hours.FirstOrDefault(h => h.DayOfWeek == date.DayOfWeek);

        if (dayHours is null || dayHours.IsClosed || dayHours.IsMaintenanceWindow)
            return [];

        var slots = new List<TimeSlot>();
        var current = dayHours.OpenTime;
        while (current + slotDuration <= dayHours.CloseTime)
        {
            slots.Add(TimeSlot.Create(date, current, current + slotDuration));
            current += slotDuration;
        }
        return slots;
    }

    public Task<bool> IsMaintenanceWindowAsync(Guid resourceId, string resourceType, DateTime date, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }
}
