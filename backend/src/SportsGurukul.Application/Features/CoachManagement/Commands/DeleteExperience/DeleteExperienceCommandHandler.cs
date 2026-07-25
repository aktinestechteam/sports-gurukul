using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeleteExperience;

public class DeleteExperienceCommandHandler : IRequestHandler<DeleteExperienceCommand, Result<Unit>>
{
    private readonly IRepository<CoachExperience> _experienceRepository;
    private readonly ICoachRepository _coachRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteExperienceCommandHandler> _logger;
    private readonly ICurrentUser _currentUser;

    public DeleteExperienceCommandHandler(
        IRepository<CoachExperience> experienceRepository,
        ICoachRepository coachRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteExperienceCommandHandler> logger,
        ICurrentUser currentUser)
    {
        _experienceRepository = experienceRepository;
        _coachRepository = coachRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<Result<Unit>> Handle(DeleteExperienceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting experience with Id: {ExperienceId}", request.ExperienceId);

        var experience = await _experienceRepository.GetByIdAsync(request.ExperienceId, cancellationToken);
        if (experience is null)
            return Result<Unit>.Failure("Experience not found.");

        var coach = await _coachRepository.GetByIdAsync(experience.CoachId, cancellationToken);
        if (coach is not null && _currentUser.Roles.Contains("Coach") && coach.UserId != _currentUser.UserId)
            return Result<Unit>.Failure("You are not authorized to modify this coach's data.");

        _experienceRepository.Remove(experience);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Experience deleted with Id: {ExperienceId}", request.ExperienceId);

        return Result<Unit>.Success(Unit.Value);
    }
}
