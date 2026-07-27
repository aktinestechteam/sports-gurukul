using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetMilestonesByProgramQuery;

public class GetMilestonesByProgramQueryHandler : IRequestHandler<GetMilestonesByProgramQuery, Result<IReadOnlyList<TrainingMilestoneDto>>>
{
    private readonly ITrainingProgramRepository _repository;
    private readonly ILogger<GetMilestonesByProgramQueryHandler> _logger;

    public GetMilestonesByProgramQueryHandler(
        ITrainingProgramRepository repository,
        ILogger<GetMilestonesByProgramQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<TrainingMilestoneDto>>> Handle(GetMilestonesByProgramQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting milestones for program ID: {ProgramId}", request.ProgramId);

        var program = await _repository.GetByIdWithDetailsAsync(request.ProgramId, cancellationToken);

        if (program == null)
        {
            return Result<IReadOnlyList<TrainingMilestoneDto>>.Failure($"Training program with ID {request.ProgramId} not found.");
        }

        var milestones = program.Milestones?.Select(TrainingMilestoneDto.MapToDto).ToList()
            ?? new List<TrainingMilestoneDto>();

        return Result<IReadOnlyList<TrainingMilestoneDto>>.Success(milestones);
    }
}
