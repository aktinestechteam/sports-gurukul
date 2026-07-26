using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.RegisterAthlete;

public class RegisterAthleteCommandHandler : IRequestHandler<RegisterAthleteCommand, Result<AcademyAthleteSummaryDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IAthleteRepository _athleteRepository;
    private readonly IAthleteAcademyRepository _athleteAcademyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterAthleteCommandHandler> _logger;

    public RegisterAthleteCommandHandler(
        IAcademyRepository academyRepository,
        IAthleteRepository athleteRepository,
        IAthleteAcademyRepository athleteAcademyRepository,
        IUnitOfWork unitOfWork,
        ILogger<RegisterAthleteCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _athleteRepository = athleteRepository;
        _athleteAcademyRepository = athleteAcademyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AcademyAthleteSummaryDto>> Handle(RegisterAthleteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering athlete {AthleteId} in academy {AcademyId}", request.AthleteId, request.AcademyId);

        var academy = await _academyRepository.GetByIdAsync(request.AcademyId, cancellationToken);
        if (academy is null)
            return Result<AcademyAthleteSummaryDto>.Failure("Academy not found.");

        if (academy.VerificationStatus != VerificationStatus.Verified)
            return Result<AcademyAthleteSummaryDto>.Failure("Academy must be verified to register athletes.");

        var athlete = await _athleteRepository.GetByIdAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
            return Result<AcademyAthleteSummaryDto>.Failure("Athlete not found.");

        if (athlete.Status != AthleteStatus.Active)
            return Result<AcademyAthleteSummaryDto>.Failure("Athlete must be active to be registered.");

        var existingRegistration = await _athleteAcademyRepository.AnyAsync(
            aa => aa.AcademyId == request.AcademyId && aa.AthleteId == request.AthleteId && aa.IsActive, cancellationToken);

        if (existingRegistration)
            return Result<AcademyAthleteSummaryDto>.Failure("Athlete is already registered in this academy.");

        var athleteAcademy = new AthleteAcademy
        {
            Id = Guid.NewGuid(),
            AcademyId = request.AcademyId,
            AthleteId = request.AthleteId,
            RegisteredDate = DateTime.UtcNow,
            IsActive = true
        };

        await _athleteAcademyRepository.AddAsync(athleteAcademy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Athlete {AthleteId} registered in academy {AcademyId}", request.AthleteId, request.AcademyId);

        var dto = new AcademyAthleteSummaryDto
        {
            Id = athleteAcademy.Id,
            AthleteId = athlete.Id,
            AthleteCode = athlete.AthleteCode,
            FullName = athlete.User.FullName,
            Email = athlete.User.Email,
            PhoneNumber = athlete.User.PhoneNumber,
            ProfileImageUrl = athlete.User.ProfileImageUrl,
            CurrentLevel = athlete.CurrentLevel.ToString(),
            Status = athlete.Status.ToString(),
            RegisteredDate = athleteAcademy.RegisteredDate
        };

        return Result<AcademyAthleteSummaryDto>.Success(dto);
    }
}
