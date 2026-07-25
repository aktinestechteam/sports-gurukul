using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.UpdateExperience;

public class UpdateExperienceCommandHandler : IRequestHandler<UpdateExperienceCommand, Result<ExperienceDto>>
{
    private readonly IRepository<CoachExperience> _coachExperienceRepository;
    private readonly ICoachRepository _coachRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateExperienceCommandHandler> _logger;
    private readonly ICurrentUser _currentUser;

    public UpdateExperienceCommandHandler(
        IRepository<CoachExperience> coachExperienceRepository,
        ICoachRepository coachRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateExperienceCommandHandler> logger,
        ICurrentUser currentUser)
    {
        _coachExperienceRepository = coachExperienceRepository;
        _coachRepository = coachRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<Result<ExperienceDto>> Handle(UpdateExperienceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating experience: {ExperienceId}", request.ExperienceId);

        var experience = await _coachExperienceRepository.GetByIdAsync(request.ExperienceId, cancellationToken);
        if (experience is null || experience.IsDeleted)
        {
            _logger.LogWarning("Experience not found: {ExperienceId}", request.ExperienceId);
            return Result<ExperienceDto>.Failure("Experience not found.");
        }

        var coach = await _coachRepository.GetByIdAsync(experience.CoachId, cancellationToken);
        if (coach is not null && _currentUser.Roles.Contains("Coach") && coach.UserId != _currentUser.UserId)
            return Result<ExperienceDto>.Failure("You are not authorized to modify this coach's data.");

        if (request.Organization is not null) experience.Organization = request.Organization;
        if (request.Role is not null) experience.Role = request.Role;
        if (request.Sport is not null) experience.Sport = request.Sport;
        if (request.StartDate.HasValue) experience.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue) experience.EndDate = request.EndDate;
        if (request.Description is not null) experience.Description = request.Description;
        experience.UpdatedAt = DateTime.UtcNow;

        _coachExperienceRepository.Update(experience);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Experience updated: {ExperienceId}", request.ExperienceId);

        var dto = new ExperienceDto
        {
            Id = experience.Id,
            Organization = experience.Organization,
            Role = experience.Role,
            Sport = experience.Sport,
            StartDate = experience.StartDate,
            EndDate = experience.EndDate,
            Description = experience.Description,
            CreatedAt = experience.CreatedAt,
            UpdatedAt = experience.UpdatedAt
        };

        return Result<ExperienceDto>.Success(dto);
    }
}
