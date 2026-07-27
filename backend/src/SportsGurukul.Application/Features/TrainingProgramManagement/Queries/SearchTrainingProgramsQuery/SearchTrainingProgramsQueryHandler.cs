using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.SearchTrainingProgramsQuery
{
    public class SearchTrainingProgramsQueryHandler : IRequestHandler<SearchTrainingProgramsQuery, Result<TrainingProgramSearchResponse>>
    {
        private readonly ITrainingProgramRepository _repository;
        private readonly ILogger<SearchTrainingProgramsQueryHandler> _logger;

        public SearchTrainingProgramsQueryHandler(
            ITrainingProgramRepository repository,
            ILogger<SearchTrainingProgramsQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<TrainingProgramSearchResponse>> Handle(SearchTrainingProgramsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Searching training programs with filters: AcademyId={AcademyId}, SportId={SportId}, Status={Status}, SearchTerm={SearchTerm}",
                request.AcademyId, request.SportId, request.Status, request.SearchTerm);

            var query = (await _repository.GetAllAsync(cancellationToken)).AsQueryable();

            if (request.AcademyId.HasValue)
            {
                query = query.Where(p => p.AcademyId == request.AcademyId.Value);
            }

            if (request.SportId.HasValue)
            {
                query = query.Where(p => p.SportId == request.SportId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<TrainingProgramStatus>(request.Status, true, out var statusEnum))
            {
                query = query.Where(p => p.Status == statusEnum);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.ToLower();
                query = query.Where(p =>
                    p.ProgramCode.ToLower().Contains(term) ||
                    p.ProgramName.ToLower().Contains(term) ||
                    (p.Description != null && p.Description.ToLower().Contains(term)));
            }

            var totalRecords = query.Count();

            var programs = query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var items = programs.Select(p => new TrainingProgramSummaryDto
            {
                Id = p.Id,
                ProgramCode = p.ProgramCode,
                ProgramName = p.ProgramName,
                SportName = p.Sport?.Name ?? string.Empty,
                AcademyName = p.Academy?.Name ?? string.Empty,
                DifficultyLevel = p.DifficultyLevel.ToString(),
                DurationWeeks = p.DurationWeeks,
                Capacity = p.Capacity,
                Status = p.Status.ToString(),
                TotalBatches = p.Batches?.Count ?? 0,
                CreatedAt = p.CreatedAt
            }).ToList();

            var response = new TrainingProgramSearchResponse
            {
                Programs = items,
                TotalCount = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize),
                PageNumber = request.Page,
                PageSize = request.PageSize
            };

            return Result<TrainingProgramSearchResponse>.Success(response);
        }
    }
}
