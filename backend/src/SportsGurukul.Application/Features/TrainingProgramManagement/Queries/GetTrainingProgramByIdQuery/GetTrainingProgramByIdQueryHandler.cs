using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Application.Common.Interfaces;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetTrainingProgramByIdQuery
{
    public class GetTrainingProgramByIdQueryHandler : IRequestHandler<GetTrainingProgramByIdQuery, Result<TrainingProgramDto>>
    {
        private readonly ITrainingProgramRepository _repository;
        private readonly ILogger<GetTrainingProgramByIdQueryHandler> _logger;

        public GetTrainingProgramByIdQueryHandler(
            ITrainingProgramRepository repository,
            ILogger<GetTrainingProgramByIdQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<TrainingProgramDto>> Handle(GetTrainingProgramByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting training program by ID: {Id}", request.Id);

            var program = await _repository.GetByIdWithDetailsAsync(request.Id, cancellationToken);

            if (program == null)
            {
                return Result<TrainingProgramDto>.Failure($"Training program with ID {request.Id} not found.");
            }

            var dto = new TrainingProgramDto
            {
                Id = program.Id,
                ProgramCode = program.ProgramCode,
                ProgramName = program.ProgramName,
                SportId = program.SportId,
                SportName = program.Sport?.Name ?? string.Empty,
                AcademyId = program.AcademyId,
                AcademyName = program.Academy?.Name ?? string.Empty,
                Description = program.Description,
                DifficultyLevel = program.DifficultyLevel.ToString(),
                MinimumAge = program.MinimumAge,
                MaximumAge = program.MaximumAge,
                DurationWeeks = program.DurationWeeks,
                Capacity = program.Capacity,
                Status = program.Status.ToString(),
                TotalBatches = program.Batches?.Count ?? 0,
                ActiveBatches = program.Batches?.Count(b => b.Status == Domain.Enums.BatchStatus.Active) ?? 0,
                RowVersion = program.RowVersion ?? [],
                CreatedAt = program.CreatedAt,
                UpdatedAt = program.UpdatedAt,
                Batches = program.Batches?.Select(b => new TrainingBatchDto
                {
                    Id = b.Id,
                    ProgramId = b.ProgramId,
                    ProgramName = program.ProgramName,
                    BatchCode = b.BatchCode,
                    CoachId = b.CoachId,
                    CoachName = b.Coach?.User?.FullName ?? string.Empty,
                    BranchId = b.BranchId,
                    BranchName = b.Branch?.BranchName ?? string.Empty,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate ?? DateTime.UtcNow,
                    MaximumSeats = b.MaximumSeats,
                    EnrollmentCount = b.Enrollments?.Count ?? 0,
                    SessionsCount = b.Sessions?.Count ?? 0,
                    Status = b.Status.ToString(),
                    RowVersion = b.RowVersion ?? [],
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                }).ToList() ?? []
            };

            return Result<TrainingProgramDto>.Success(dto);
        }
    }
}
