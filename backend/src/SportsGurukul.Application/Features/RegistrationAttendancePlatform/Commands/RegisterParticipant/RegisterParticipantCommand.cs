using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.RegisterParticipant;

public class RegisterParticipantCommand : IRequest<Result<PlatformRegistrationDto>>
{
    public ProgramType ProgramType { get; set; }
    public Guid ProgramId { get; set; }
    public Guid? AthleteId { get; set; }
    public Guid? UserId { get; set; }
    public EventRegistrationType RegistrationType { get; set; }
    public decimal? AmountPaid { get; set; }
    public string? PaymentReference { get; set; }
    public string? Notes { get; set; }
}
