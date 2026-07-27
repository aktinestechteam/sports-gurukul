using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.IssueCertificate;

public class IssueCertificateCommandHandler : IRequestHandler<IssueCertificateCommand, Result<DTOs.CertificateDto>>
{
    private readonly ITrainingBatchRepository _batchRepository;
    private readonly ILogger<IssueCertificateCommandHandler> _logger;

    private static readonly HashSet<string> ValidCertificateTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        nameof(CertificateType.Completion),
        nameof(CertificateType.Participation),
        nameof(CertificateType.Merit),
        nameof(CertificateType.Excellence)
    };

    public IssueCertificateCommandHandler(
        ITrainingBatchRepository batchRepository,
        ILogger<IssueCertificateCommandHandler> logger)
    {
        _batchRepository = batchRepository;
        _logger = logger;
    }

    public async Task<Result<DTOs.CertificateDto>> Handle(IssueCertificateCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Issuing {CertificateType} certificate for enrollment {EnrollmentId}", request.CertificateType, request.EnrollmentId);

        if (!ValidCertificateTypes.Contains(request.CertificateType))
        {
            _logger.LogWarning("Invalid certificate type: {CertificateType}. Valid types: {ValidTypes}", request.CertificateType, string.Join(", ", ValidCertificateTypes));
            return Result<DTOs.CertificateDto>.Failure($"Invalid certificate type. Valid types are: {string.Join(", ", ValidCertificateTypes)}");
        }

        TrainingEnrollment? enrollment = null;
        TrainingBatch? foundBatch = null;

        var batches = await _batchRepository.GetAllAsync(cancellationToken);
        foreach (var batch in batches)
        {
            var details = await _batchRepository.GetByIdWithDetailsAsync(batch.Id, cancellationToken);
            enrollment = details?.Enrollments?.FirstOrDefault(e => e.Id == request.EnrollmentId);
            if (enrollment is not null)
            {
                foundBatch = details;
                break;
            }
        }

        if (enrollment is null || foundBatch is null)
        {
            _logger.LogWarning("Enrollment {EnrollmentId} not found", request.EnrollmentId);
            return Result<DTOs.CertificateDto>.Failure("Enrollment not found");
        }

        if (enrollment.Status != EnrollmentStatus.Completed)
        {
            _logger.LogWarning("Enrollment {EnrollmentId} is not completed. Current status: {Status}", request.EnrollmentId, enrollment.Status);
            return Result<DTOs.CertificateDto>.Failure("Certificate can only be issued for completed enrollments");
        }

        var certificateNumber = GenerateCertificateNumber();

        var certificate = new TrainingCertificate
        {
            Id = Guid.NewGuid(),
            EnrollmentId = request.EnrollmentId,
            CertificateType = Enum.Parse<CertificateType>(request.CertificateType, ignoreCase: true),
            CertificateNumber = certificateNumber,
            IssuedDate = DateTime.UtcNow,
            FileUrl = request.FileUrl,
            CreatedAt = DateTime.UtcNow
        };

        enrollment.Certificates ??= new List<TrainingCertificate>();
        enrollment.Certificates.Add(certificate);
        _batchRepository.Update(foundBatch);

        var dto = new DTOs.CertificateDto
        {
            Id = certificate.Id,
            EnrollmentId = certificate.EnrollmentId,
            CertificateType = certificate.CertificateType.ToString(),
            CertificateNumber = certificate.CertificateNumber,
            IssuedDate = certificate.IssuedDate,
            FileUrl = certificate.FileUrl,
            CreatedAt = certificate.CreatedAt
        };

        _logger.LogInformation("Certificate {CertificateId} (Number: {CertificateNumber}) successfully issued for enrollment {EnrollmentId}", certificate.Id, certificateNumber, request.EnrollmentId);
        return Result<DTOs.CertificateDto>.Success(dto);
    }

    private static string GenerateCertificateNumber()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random();
        var randomPart = random.Next(100000, 999999).ToString();
        return $"CERT-{datePart}-{randomPart}";
    }
}
