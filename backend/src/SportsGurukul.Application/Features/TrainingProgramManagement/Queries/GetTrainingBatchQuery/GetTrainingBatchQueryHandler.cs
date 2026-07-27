using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Application.Common.Interfaces;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetTrainingBatchQuery
{
    public class GetTrainingBatchQueryHandler : IRequestHandler<GetTrainingBatchQuery, Result<TrainingBatchDto>>
    {
        private readonly ITrainingBatchRepository _repository;
        private readonly ILogger<GetTrainingBatchQueryHandler> _logger;

        public GetTrainingBatchQueryHandler(
            ITrainingBatchRepository repository,
            ILogger<GetTrainingBatchQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<TrainingBatchDto>> Handle(GetTrainingBatchQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting training batch by ID: {Id}", request.Id);

            var batch = await _repository.GetByIdWithDetailsAsync(request.Id, cancellationToken);

            if (batch == null)
            {
                return Result<TrainingBatchDto>.Failure($"Training batch with ID {request.Id} not found.");
            }

            var dto = new TrainingBatchDto
            {
                Id = batch.Id,
                ProgramId = batch.ProgramId,
                ProgramName = batch.Program?.ProgramName ?? string.Empty,
                BatchCode = batch.BatchCode,
                CoachId = batch.CoachId,
                CoachName = batch.Coach?.User?.FullName ?? string.Empty,
                BranchId = batch.BranchId,
                BranchName = batch.Branch?.BranchName ?? string.Empty,
                StartDate = batch.StartDate,
                EndDate = batch.EndDate ?? DateTime.UtcNow,
                MaximumSeats = batch.MaximumSeats,
                EnrollmentCount = batch.Enrollments?.Count(e => e.Status == Domain.Enums.EnrollmentStatus.Active) ?? 0,
                SessionsCount = batch.Sessions?.Count ?? 0,
                Status = batch.Status.ToString(),
                RowVersion = batch.RowVersion ?? [],
                CreatedAt = batch.CreatedAt,
                UpdatedAt = batch.UpdatedAt
            };

            return Result<TrainingBatchDto>.Success(dto);
        }
    }
}
