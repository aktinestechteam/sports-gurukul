using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public class TimeSlotGenerator : ITimeSlotGenerator
{
    private readonly ILogger<TimeSlotGenerator> _logger;

    public TimeSlotGenerator(ILogger<TimeSlotGenerator> logger) => _logger = logger;

    public IReadOnlyList<TimeSlot> GenerateDailySlots(DateTime date, TimeOnly startTime, TimeOnly endTime, TimeSpan slotDuration, TimeSpan? buffer = null)
    {
        var slots = new List<TimeSlot>();
        var current = startTime.ToTimeSpan();
        var final = endTime.ToTimeSpan();
        var effectiveDuration = slotDuration + (buffer ?? TimeSpan.Zero);

        while (current + slotDuration <= final)
        {
            slots.Add(TimeSlot.Create(date, current, current + slotDuration));
            current += effectiveDuration;
        }

        _logger.LogDebug("Generated {Count} daily slots for {Date}", slots.Count, date.Date);
        return slots;
    }

    public IReadOnlyList<TimeSlot> GenerateSlotsForDateRange(DateTime startDate, DateTime endDate, TimeOnly startTime, TimeOnly endTime, TimeSpan slotDuration, TimeSpan? buffer = null)
    {
        var allSlots = new List<TimeSlot>();
        var current = startDate.Date;

        while (current <= endDate.Date)
        {
            allSlots.AddRange(GenerateDailySlots(current, startTime, endTime, slotDuration, buffer));
            current = current.AddDays(1);
        }

        _logger.LogDebug("Generated {Count} slots for range {Start} to {End}", allSlots.Count, startDate.Date, endDate.Date);
        return allSlots;
    }

    public IReadOnlyList<TimeSlot> GenerateSlotsExcluding(DateTime date, TimeOnly startTime, TimeOnly endTime, TimeSpan slotDuration, IReadOnlyList<TimeSlot> existingSlots, TimeSpan? buffer = null)
    {
        var allSlots = GenerateDailySlots(date, startTime, endTime, slotDuration, buffer);
        return SubtractSlots(allSlots, existingSlots);
    }

    public IReadOnlyList<TimeSlot> MergeSlots(IReadOnlyList<TimeSlot> slots)
    {
        if (slots.Count == 0) return [];

        var sorted = slots.OrderBy(s => s.Date).ThenBy(s => s.StartTime).ToList();
        var merged = new List<TimeSlot> { sorted[0] };

        for (int i = 1; i < sorted.Count; i++)
        {
            var last = merged[^1];
            if (sorted[i].Date.Date == last.Date.Date && sorted[i].StartTime <= last.EndTime)
            {
                merged[^1] = last with { EndTime = TimeSpan.FromTicks(Math.Max(last.EndTime.Ticks, sorted[i].EndTime.Ticks)) };
            }
            else
            {
                merged.Add(sorted[i]);
            }
        }

        return merged;
    }

    public IReadOnlyList<TimeSlot> SubtractSlots(IReadOnlyList<TimeSlot> available, IReadOnlyList<TimeSlot> blocked)
    {
        var result = new List<TimeSlot>(available);

        foreach (var block in blocked)
        {
            var newResult = new List<TimeSlot>();
            foreach (var slot in result)
            {
                if (!slot.Overlaps(block))
                {
                    newResult.Add(slot);
                    continue;
                }

                if (slot.StartTime < block.StartTime)
                {
                    newResult.Add(slot with { EndTime = block.StartTime });
                }

                if (slot.EndTime > block.EndTime)
                {
                    newResult.Add(slot with { StartTime = block.EndTime });
                }
            }
            result = newResult;
        }

        return result.Where(s => s.DurationMinutes > 0).ToList();
    }
}
