using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetAthleteEnrollmentsQuery
{
    public class GetAthleteEnrollmentsQueryHandler : IRequestHandler<GetAthleteEnrollmentsQuery, Result<IReadOnlyList<EnrollmentDto>>>
    {
        private readonly ITrainingBatchRepository _batchRepository;
        private readonly ILogger<GetAthleteEnrollmentsQueryHandler> _logger;

        public GetAthleteEnrollmentsQueryHandler(
            ITrainingBatchRepository batchRepository,
            ILogger<GetAthleteEnrollmentsQueryHandler> logger)
        {
            _batchRepository = batchRepository;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyList<EnrollmentDto>>> Handle(GetAthleteEnrollmentsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting enrollments for athlete: {AthleteId}", request.AthleteId);

            var batches = await _batchRepository.GetAllAsync(cancellationToken);
            var enrollments = batches
                .SelectMany(b => b.Enrollments ?? [])
                .Where(e => e.AthleteId == request.AthleteId)
                .ToList();

            var dtos = enrollments.Select(e => new EnrollmentDto
            {
                Id = e.Id,
                BatchId = e.BatchId,
                BatchCode = e.Batch?.BatchCode ?? string.Empty,
                ProgramName = e.Batch?.Program?.ProgramName ?? string.Empty,
                AthleteId = e.AthleteId,
                AthleteName = e.Athlete?.User?.FullName ?? string.Empty,
                AthleteCode = e.Athlete?.AthleteCode ?? string.Empty,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status.ToString(),
                Progress = e.Progress != null ? new TrainingProgressDto
                {
                    Id = e.Progress.Id,
                    EnrollmentId = e.Progress.EnrollmentId,
                    CurrentLevel = e.Progress.CurrentLevel.ToString(),
                    CompletedPercentage = e.Progress.CompletedPercentage,
                    OverallRating = e.Progress.OverallRating,
                    RowVersion = e.Progress.RowVersion ?? [],
                    CreatedAt = e.Progress.CreatedAt,
                    UpdatedAt = e.Progress.UpdatedAt
                } : null,
                Certificates = e.Certificates?.Select(c => new CertificateDto
                {
                    Id = c.Id,
                    EnrollmentId = c.EnrollmentId,
                    CertificateType = c.CertificateType.ToString(),
                    CertificateNumber = c.CertificateNumber,
                    IssuedDate = c.IssuedDate,
                    FileUrl = c.FileUrl,
                    CreatedAt = c.CreatedAt
                }).ToList() ?? [],
                RowVersion = e.RowVersion ?? [],
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).ToList();

            return Result<IReadOnlyList<EnrollmentDto>>.Success(dtos);
        }
    }
}
