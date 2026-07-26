using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class AcademyOperatingHours : BaseEntity
{
    public Guid AcademyId { get; set; }
    public TimeOnly? MondayOpening { get; set; }
    public TimeOnly? MondayClosing { get; set; }
    public TimeOnly? TuesdayOpening { get; set; }
    public TimeOnly? TuesdayClosing { get; set; }
    public TimeOnly? WednesdayOpening { get; set; }
    public TimeOnly? WednesdayClosing { get; set; }
    public TimeOnly? ThursdayOpening { get; set; }
    public TimeOnly? ThursdayClosing { get; set; }
    public TimeOnly? FridayOpening { get; set; }
    public TimeOnly? FridayClosing { get; set; }
    public TimeOnly? SaturdayOpening { get; set; }
    public TimeOnly? SaturdayClosing { get; set; }
    public TimeOnly? SundayOpening { get; set; }
    public TimeOnly? SundayClosing { get; set; }
    public string? HolidaySchedule { get; set; }

    public Academy Academy { get; set; } = null!;
}
