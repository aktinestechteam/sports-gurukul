namespace SportsGurukul.Application.Features.SharedScheduling.Models;

public sealed record TimeSlot
{
    public DateTime Date { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public int DurationMinutes => (int)(EndTime - StartTime).TotalMinutes;
    public bool CrossesMidnight => EndTime < StartTime;
    
    public static TimeSlot Create(DateTime date, TimeSpan start, TimeSpan end) => new() { Date = date, StartTime = start, EndTime = end };
    
    public bool Overlaps(TimeSlot other) => Date.Date == other.Date.Date && StartTime < other.EndTime && EndTime > other.StartTime;
    
    public TimeSlot Shift(TimeSpan offset) => this with { StartTime = StartTime + offset, EndTime = EndTime + offset };
    
    public IReadOnlyList<TimeSlot> SplitAtMidnight()
    {
        if (!CrossesMidnight) return [this];
        var nightPart = new TimeSlot { Date = Date, StartTime = StartTime, EndTime = new TimeSpan(23, 59, 59) };
        var morningPart = new TimeSlot { Date = Date.AddDays(1), StartTime = TimeSpan.Zero, EndTime = EndTime };
        return [nightPart, morningPart];
    }
}
