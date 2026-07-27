using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Application.Common.Interfaces;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetTrainingProgressQuery
{
    public class GetTrainingProgressQueryHandler : IRequestHandler<GetTrainingProgressQuery, Result<TrainingProgressDto>>
    {
        private readonly ITrainingProgressRepository _repository;
        private readonly ILogger<GetTrainingProgressQueryHandler> _logger;

        public GetTrainingProgressQueryHandler(
            ITrainingProgressRepository repository,
            ILogger<GetTrainingProgressQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<TrainingProgressDto>> Handle(GetTrainingProgressQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting training progress for enrollment: {EnrollmentId}", request.EnrollmentId);

            var progress = await _repository.GetByEnrollmentIdAsync(request.EnrollmentId, cancellationToken);

            if (progress == null)
            {
                return Result<TrainingProgressDto>.Failure($"Training progress for enrollment {request.EnrollmentId} not found.");
            }

            var dto = new TrainingProgressDto
            {
                Id = progress.Id,
                EnrollmentId = progress.EnrollmentId,
                CurrentLevel = progress.CurrentLevel.ToString(),
                CompletedPercentage = progress.CompletedPercentage,
                OverallRating = progress.OverallRating,
                RowVersion = progress.RowVersion ?? [],
                CreatedAt = progress.CreatedAt,
                UpdatedAt = progress.UpdatedAt
            };

            return Result<TrainingProgressDto>.Success(dto);
        }
    }
}
