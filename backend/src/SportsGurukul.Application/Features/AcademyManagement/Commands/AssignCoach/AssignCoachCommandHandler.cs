using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.AssignCoach;

public class AssignCoachCommandHandler : IRequestHandler<AssignCoachCommand, Result<AcademyCoachSummaryDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly ICoachRepository _coachRepository;
    private readonly ICoachAcademyRepository _coachAcademyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignCoachCommandHandler> _logger;

    public AssignCoachCommandHandler(
        IAcademyRepository academyRepository,
        ICoachRepository coachRepository,
        ICoachAcademyRepository coachAcademyRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignCoachCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _coachRepository = coachRepository;
        _coachAcademyRepository = coachAcademyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AcademyCoachSummaryDto>> Handle(AssignCoachCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning coach {CoachId} to academy {AcademyId}", request.CoachId, request.AcademyId);

        var academy = await _academyRepository.GetByIdAsync(request.AcademyId, cancellationToken);
        if (academy is null)
            return Result<AcademyCoachSummaryDto>.Failure("Academy not found.");

        if (academy.VerificationStatus != VerificationStatus.Verified)
            return Result<AcademyCoachSummaryDto>.Failure("Academy must be verified to assign coaches.");

        var coach = await _coachRepository.GetByIdAsync(request.CoachId, cancellationToken);
        if (coach is null)
            return Result<AcademyCoachSummaryDto>.Failure("Coach not found.");

        if (coach.Status != CoachStatus.Active)
            return Result<AcademyCoachSummaryDto>.Failure("Coach must be active to be assigned.");

        if (coach.VerificationStatus != VerificationStatus.Verified)
            return Result<AcademyCoachSummaryDto>.Failure("Coach must be verified to be assigned.");

        var existingAssignment = await _coachAcademyRepository.AnyAsync(
            ca => ca.AcademyId == request.AcademyId && ca.CoachId == request.CoachId && ca.IsActive, cancellationToken);

        if (existingAssignment)
            return Result<AcademyCoachSummaryDto>.Failure("Coach is already assigned to this academy.");

        var coachAcademy = new CoachAcademy
        {
            Id = Guid.NewGuid(),
            AcademyId = request.AcademyId,
            CoachId = request.CoachId,
            AssignedDate = DateTime.UtcNow,
            IsActive = true
        };

        await _coachAcademyRepository.AddAsync(coachAcademy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Coach {CoachId} assigned to academy {AcademyId}", request.CoachId, request.AcademyId);

        var dto = new AcademyCoachSummaryDto
        {
            Id = coachAcademy.Id,
            CoachId = coach.Id,
            CoachCode = coach.CoachCode,
            FullName = coach.User.FullName,
            Email = coach.User.Email,
            PhoneNumber = coach.User.PhoneNumber,
            ProfileImageUrl = coach.User.ProfileImageUrl,
            CoachingLevel = coach.CoachingLevel.ToString(),
            Status = coach.Status.ToString(),
            VerificationStatus = coach.VerificationStatus.ToString(),
            YearsOfExperience = coach.YearsOfExperience,
            AssignedDate = coachAcademy.AssignedDate
        };

        return Result<AcademyCoachSummaryDto>.Success(dto);
    }
}
