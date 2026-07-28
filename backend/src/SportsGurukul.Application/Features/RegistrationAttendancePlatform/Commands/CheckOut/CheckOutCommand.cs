using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.CheckOut;

public class CheckOutCommand : IRequest<Result<PlatformAttendanceDto>>
{
    public ProgramType ProgramType { get; set; }
    public Guid ProgramId { get; set; }
    public Guid ParticipantId { get; set; }
    public Guid? SessionId { get; set; }
    public string? Remarks { get; set; }
}
