using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.CompleteMilestone;

public class CompleteMilestoneCommandHandler : IRequestHandler<CompleteMilestoneCommand, Result<bool>>
{
    private readonly ITrainingProgramRepository _programRepository;
    private readonly ILogger<CompleteMilestoneCommandHandler> _logger;

    public CompleteMilestoneCommandHandler(
        ITrainingProgramRepository programRepository,
        ILogger<CompleteMilestoneCommandHandler> logger)
    {
        _programRepository = programRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(CompleteMilestoneCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing milestone {MilestoneId} for program {ProgramId}", request.MilestoneId, request.ProgramId);

        var program = await _programRepository.GetByIdAsync(request.ProgramId, cancellationToken);
        if (program is null)
        {
            _logger.LogWarning("Program {ProgramId} not found", request.ProgramId);
            return Result<bool>.Failure("Program not found");
        }

        var milestone = program.Milestones?.FirstOrDefault(m => m.Id == request.MilestoneId);
        if (milestone is null)
        {
            _logger.LogWarning("Milestone {MilestoneId} not found in program {ProgramId}", request.MilestoneId, request.ProgramId);
            return Result<bool>.Failure("Milestone not found in the specified program");
        }

        if (milestone.IsCompleted)
        {
            _logger.LogWarning("Milestone {MilestoneId} is already completed", request.MilestoneId);
            return Result<bool>.Failure("Milestone is already completed");
        }

        milestone.IsCompleted = true;
        _programRepository.Update(program);

        _logger.LogInformation("Milestone '{MilestoneName}' (ID: {MilestoneId}) successfully completed for program {ProgramId}", milestone.MilestoneName, request.MilestoneId, request.ProgramId);
        return Result<bool>.Success(true);
    }
}
