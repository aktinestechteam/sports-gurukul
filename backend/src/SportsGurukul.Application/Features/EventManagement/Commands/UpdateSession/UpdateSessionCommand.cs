using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.UpdateSession;

public class UpdateSessionCommand : IRequest<Result<EventSessionDto>>
{
    public Guid SessionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime SessionDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public Guid? VenueId { get; set; }
    public Guid? SpeakerId { get; set; }
    public Guid? CoachId { get; set; }
    public int? Capacity { get; set; }
    public bool IsBreak { get; set; }
    public string? Notes { get; set; }
}
