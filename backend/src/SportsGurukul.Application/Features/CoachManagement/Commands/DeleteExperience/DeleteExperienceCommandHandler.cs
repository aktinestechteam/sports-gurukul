using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeleteExperience;

public class DeleteExperienceCommandHandler : IRequestHandler<DeleteExperienceCommand, Result<Unit>>
{
    private readonly IRepository<CoachExperience> _experienceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteExperienceCommandHandler> _logger;

    public DeleteExperienceCommandHandler(
        IRepository<CoachExperience> experienceRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteExperienceCommandHandler> logger)
    {
        _experienceRepository = experienceRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteExperienceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting experience with Id: {ExperienceId}", request.ExperienceId);

        var experience = await _experienceRepository.GetByIdAsync(request.ExperienceId, cancellationToken);
        if (experience is null)
            return Result<Unit>.Failure("Experience not found.");

        _experienceRepository.Remove(experience);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Experience deleted with Id: {ExperienceId}", request.ExperienceId);

        return Result<Unit>.Success(Unit.Value);
    }
}
