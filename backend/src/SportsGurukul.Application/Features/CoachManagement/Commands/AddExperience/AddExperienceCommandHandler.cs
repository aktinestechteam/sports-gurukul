using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.AddExperience;

public class AddExperienceCommandHandler : IRequestHandler<AddExperienceCommand, Result<ExperienceDto>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly IRepository<CoachExperience> _coachExperienceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddExperienceCommandHandler> _logger;

    public AddExperienceCommandHandler(
        ICoachRepository coachRepository,
        IRepository<CoachExperience> coachExperienceRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddExperienceCommandHandler> logger)
    {
        _coachRepository = coachRepository;
        _coachExperienceRepository = coachExperienceRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ExperienceDto>> Handle(AddExperienceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding experience to coach: {CoachId}", request.CoachId);

        var coach = await _coachRepository.GetByIdAsync(request.CoachId, cancellationToken);
        if (coach is null)
        {
            _logger.LogWarning("Coach not found: {CoachId}", request.CoachId);
            return Result<ExperienceDto>.Failure("Coach not found.");
        }

        var experience = new CoachExperience
        {
            Id = Guid.NewGuid(),
            CoachId = request.CoachId,
            Organization = request.Organization,
            Role = request.Role,
            Sport = request.Sport,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Description = request.Description
        };

        await _coachExperienceRepository.AddAsync(experience, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Experience added: {ExperienceId}, {CoachId}", experience.Id, request.CoachId);

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
