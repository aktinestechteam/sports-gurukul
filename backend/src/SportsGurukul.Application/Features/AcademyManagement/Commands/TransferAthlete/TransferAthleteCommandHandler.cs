using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.TransferAthlete;

public class TransferAthleteCommandHandler : IRequestHandler<TransferAthleteCommand, Result<AcademyAthleteSummaryDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IAthleteRepository _athleteRepository;
    private readonly IAthleteAcademyRepository _athleteAcademyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransferAthleteCommandHandler> _logger;

    public TransferAthleteCommandHandler(
        IAcademyRepository academyRepository,
        IAthleteRepository athleteRepository,
        IAthleteAcademyRepository athleteAcademyRepository,
        IUnitOfWork unitOfWork,
        ILogger<TransferAthleteCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _athleteRepository = athleteRepository;
        _athleteAcademyRepository = athleteAcademyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AcademyAthleteSummaryDto>> Handle(TransferAthleteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Transferring athlete {AthleteId} from academy {FromAcademyId} to academy {ToAcademyId}",
            request.AthleteId, request.FromAcademyId, request.ToAcademyId);

        var fromAcademy = await _academyRepository.GetByIdAsync(request.FromAcademyId, cancellationToken);
        if (fromAcademy is null)
            return Result<AcademyAthleteSummaryDto>.Failure("Source academy not found.");

        if (fromAcademy.VerificationStatus != VerificationStatus.Verified)
            return Result<AcademyAthleteSummaryDto>.Failure("Source academy must be verified.");

        var toAcademy = await _academyRepository.GetByIdAsync(request.ToAcademyId, cancellationToken);
        if (toAcademy is null)
            return Result<AcademyAthleteSummaryDto>.Failure("Target academy not found.");

        if (toAcademy.VerificationStatus != VerificationStatus.Verified)
            return Result<AcademyAthleteSummaryDto>.Failure("Target academy must be verified.");

        var athlete = await _athleteRepository.GetByIdAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
            return Result<AcademyAthleteSummaryDto>.Failure("Athlete not found.");

        var currentRegistration = await _athleteAcademyRepository.GetByAcademyAndAthleteAsync(
            request.FromAcademyId, request.AthleteId, cancellationToken);

        if (currentRegistration is null || !currentRegistration.IsActive)
            return Result<AcademyAthleteSummaryDto>.Failure("Athlete is not currently registered in the source academy.");

        var existingTargetRegistration = await _athleteAcademyRepository.AnyAsync(
            aa => aa.AcademyId == request.ToAcademyId && aa.AthleteId == request.AthleteId && aa.IsActive, cancellationToken);

        if (existingTargetRegistration)
            return Result<AcademyAthleteSummaryDto>.Failure("Athlete is already registered in the target academy.");

        currentRegistration.IsActive = false;
        currentRegistration.UpdatedAt = DateTime.UtcNow;
        _athleteAcademyRepository.Update(currentRegistration);

        var newRegistration = new AthleteAcademy
        {
            Id = Guid.NewGuid(),
            AcademyId = request.ToAcademyId,
            AthleteId = request.AthleteId,
            RegisteredDate = DateTime.UtcNow,
            IsActive = true
        };

        await _athleteAcademyRepository.AddAsync(newRegistration, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Athlete {AthleteId} transferred from academy {FromAcademyId} to academy {ToAcademyId}",
            request.AthleteId, request.FromAcademyId, request.ToAcademyId);

        var dto = new AcademyAthleteSummaryDto
        {
            Id = newRegistration.Id,
            AthleteId = athlete.Id,
            AthleteCode = athlete.AthleteCode,
            FullName = athlete.User.FullName,
            Email = athlete.User.Email,
            PhoneNumber = athlete.User.PhoneNumber,
            ProfileImageUrl = athlete.User.ProfileImageUrl,
            CurrentLevel = athlete.CurrentLevel.ToString(),
            Status = athlete.Status.ToString(),
            RegisteredDate = newRegistration.RegisteredDate
        };

        return Result<AcademyAthleteSummaryDto>.Success(dto);
    }
}
