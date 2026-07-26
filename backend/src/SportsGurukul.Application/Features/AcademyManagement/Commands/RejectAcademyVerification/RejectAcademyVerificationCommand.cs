using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.RejectAcademyVerification;

public class RejectAcademyVerificationCommand : IRequest<Result<AcademyDto>>
{
    public Guid AcademyId { get; set; }
    public string Remarks { get; set; } = string.Empty;
}
