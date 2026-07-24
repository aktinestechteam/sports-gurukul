using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.VerifyCertification;

public class VerifyCertificationCommand : IRequest<Result<CertificationDto>>
{
    public Guid CertificationId { get; set; }
    public VerificationStatus Status { get; set; }
}
