using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.CheckIn;

public class CheckInCommand : IRequest<Result<PlatformAttendanceDto>>
{
    public ProgramType ProgramType { get; set; }
    public Guid ProgramId { get; set; }
    public Guid ParticipantId { get; set; }
    public Guid? SessionId { get; set; }
    public string? QrCodeData { get; set; }
    public bool IsManual { get; set; }
    public string? Remarks { get; set; }
}
