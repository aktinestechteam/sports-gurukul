using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.AssignAthlete;

public class AssignAthleteCommandHandler : IRequestHandler<AssignAthleteCommand, Result<AssignedAthleteDto>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly IAthleteRepository _athleteRepository;
    private readonly IRepository<CoachAthlete> _coachAthleteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignAthleteCommandHandler> _logger;

    public AssignAthleteCommandHandler(
        ICoachRepository coachRepository,
        IAthleteRepository athleteRepository,
        IRepository<CoachAthlete> coachAthleteRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignAthleteCommandHandler> logger)
    {
        _coachRepository = coachRepository;
        _athleteRepository = athleteRepository;
        _coachAthleteRepository = coachAthleteRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AssignedAthleteDto>> Handle(AssignAthleteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning athlete {AthleteId} to coach {CoachId}", request.AthleteId, request.CoachId);

        var coach = await _coachRepository.GetByIdAsync(request.CoachId, cancellationToken);
        if (coach is null)
            return Result<AssignedAthleteDto>.Failure("Coach not found.");

        if (coach.Status != CoachStatus.Active)
            return Result<AssignedAthleteDto>.Failure("Coach must be active to assign athletes.");

        if (coach.VerificationStatus != VerificationStatus.Verified)
            return Result<AssignedAthleteDto>.Failure("Coach must be verified to assign athletes.");

        var athlete = await _athleteRepository.GetByIdAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
            return Result<AssignedAthleteDto>.Failure("Athlete not found.");

        if (athlete.Status != AthleteStatus.Active)
            return Result<AssignedAthleteDto>.Failure("Athlete must be active to be assigned.");

        var existingAssignment = await _coachAthleteRepository.AnyAsync(
            ca => ca.CoachId == request.CoachId && ca.AthleteId == request.AthleteId && ca.IsActive, cancellationToken);

        if (existingAssignment)
            return Result<AssignedAthleteDto>.Failure("Athlete is already assigned to this coach.");

        var coachAthlete = new CoachAthlete
        {
            Id = Guid.NewGuid(),
            CoachId = request.CoachId,
            AthleteId = request.AthleteId,
            AssignedDate = DateTime.UtcNow,
            IsActive = true
        };

        await _coachAthleteRepository.AddAsync(coachAthlete, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Athlete {AthleteId} assigned to coach {CoachId}", request.AthleteId, request.CoachId);

        var dto = new AssignedAthleteDto
        {
            Id = coachAthlete.Id,
            AthleteId = athlete.Id,
            AthleteCode = athlete.AthleteCode,
            FullName = athlete.User.FullName,
            Email = athlete.User.Email,
            PhoneNumber = athlete.User.PhoneNumber,
            ProfileImageUrl = athlete.User.ProfileImageUrl,
            CurrentLevel = athlete.CurrentLevel.ToString(),
            Status = athlete.Status.ToString(),
            AssignedDate = coachAthlete.AssignedDate
        };

        return Result<AssignedAthleteDto>.Success(dto);
    }
}
