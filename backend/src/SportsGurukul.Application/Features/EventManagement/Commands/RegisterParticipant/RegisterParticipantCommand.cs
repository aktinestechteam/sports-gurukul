using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.RegisterParticipant;

public class RegisterParticipantCommand : IRequest<Result<RegistrationDto>>
{
    public Guid EventId { get; set; }
    public Guid? AthleteId { get; set; }
    public Guid? UserId { get; set; }
    public decimal? AmountPaid { get; set; }
    public string? PaymentReference { get; set; }
    public string? Notes { get; set; }
}
