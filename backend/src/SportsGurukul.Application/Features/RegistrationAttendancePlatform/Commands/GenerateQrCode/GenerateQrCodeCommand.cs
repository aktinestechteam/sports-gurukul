using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.GenerateQrCode;

public class GenerateQrCodeCommand : IRequest<Result<PlatformQrCodeDto>>
{
    public QrCodeType Type { get; set; }
    public ProgramType ProgramType { get; set; }
    public Guid ProgramId { get; set; }
    public Guid ParticipantId { get; set; }
}
