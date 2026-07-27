using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.IssueCertificate;

public record IssueCertificateCommand : IRequest<Result<DTOs.CertificateDto>>
{
    public Guid EnrollmentId { get; init; }
    public string CertificateType { get; init; } = string.Empty;
    public string? FileUrl { get; init; }
}
