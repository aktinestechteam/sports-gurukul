using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;

public class PlatformCapacityDto
{
    public ProgramType ProgramType { get; set; }
    public Guid ProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public int? MaxCapacity { get; set; }
    public int CurrentCount { get; set; }
    public int AvailableSlots { get; set; }
    public bool IsFull { get; set; }
    public int WaitlistCount { get; set; }
    public bool WaitlistEnabled { get; set; }
}
