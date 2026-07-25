using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.Commands.CreateCoach;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.UpdateCoachProfile;

public class UpdateCoachProfileCommandHandler : IRequestHandler<UpdateCoachProfileCommand, Result<CoachDto>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCoachProfileCommandHandler> _logger;
    private readonly ICurrentUser _currentUser;

    public UpdateCoachProfileCommandHandler(
        ICoachRepository coachRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateCoachProfileCommandHandler> logger,
        ICurrentUser currentUser)
    {
        _coachRepository = coachRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<Result<CoachDto>> Handle(UpdateCoachProfileCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating coach profile for CoachId: {CoachId}", request.CoachId);

        var coach = await _coachRepository.GetByIdWithDetailsAsync(request.CoachId, cancellationToken);
        if (coach is null)
            return Result<CoachDto>.Failure("Coach not found.");

        if (_currentUser.Roles.Contains("Coach") && coach.UserId != _currentUser.UserId)
            return Result<CoachDto>.Failure("You are not authorized to modify this coach's data.");

        if (request.Biography is not null)
            coach.Biography = request.Biography;

        if (request.YearsOfExperience.HasValue)
            coach.YearsOfExperience = request.YearsOfExperience.Value;

        if (request.CurrentOrganization is not null)
            coach.CurrentOrganization = request.CurrentOrganization;

        if (request.HighestQualification is not null)
            coach.HighestQualification = request.HighestQualification;

        if (request.PreferredLanguage is not null)
            coach.PreferredLanguage = request.PreferredLanguage;

        if (request.CoachingLevel.HasValue)
            coach.CoachingLevel = request.CoachingLevel.Value;

        coach.UpdatedAt = DateTime.UtcNow;

        _coachRepository.Update(coach);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Coach profile updated for CoachId: {CoachId}", request.CoachId);

        return Result<CoachDto>.Success(CreateCoachCommandHandler.MapToDto(coach, coach.User));
    }
}
