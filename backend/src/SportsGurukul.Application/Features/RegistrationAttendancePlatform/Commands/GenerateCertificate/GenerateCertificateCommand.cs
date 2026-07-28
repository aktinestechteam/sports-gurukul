using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.GenerateCertificate;

public class GenerateCertificateCommand : IRequest<Result<PlatformCertificateDto>>
{
    public ProgramType ProgramType { get; set; }
    public Guid ProgramId { get; set; }
    public Guid ParticipantId { get; set; }
    public double AttendanceRate { get; set; }
    public double? AverageScore { get; set; }
    public string? IssuedBy { get; set; }
}
