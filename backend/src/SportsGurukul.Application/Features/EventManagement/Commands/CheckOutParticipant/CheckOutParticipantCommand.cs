using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.CheckOutParticipant;

public class CheckOutParticipantCommand : IRequest<Result<AttendanceDto>>
{
    public Guid AttendanceId { get; set; }
}
