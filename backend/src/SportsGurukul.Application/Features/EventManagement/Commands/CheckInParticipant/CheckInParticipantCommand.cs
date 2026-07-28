using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.CheckInParticipant;

public class CheckInParticipantCommand : IRequest<Result<AttendanceDto>>
{
    public Guid EventId { get; set; }
    public Guid? SessionId { get; set; }
    public Guid ParticipantId { get; set; }
}
