using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateAthlete;

public class UpdateAthleteCommandHandler : IRequestHandler<UpdateAthleteCommand, Result<AthleteDto>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateAthleteCommandHandler> _logger;

    public UpdateAthleteCommandHandler(
        IAthleteRepository athleteRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateAthleteCommandHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AthleteDto>> Handle(UpdateAthleteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating athlete: {AthleteId}", request.AthleteId);

        var athlete = await _athleteRepository.GetByIdWithDetailsAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete not found: {AthleteId}", request.AthleteId);
            return Result<AthleteDto>.Failure("Athlete not found.");
        }

        if (request.CurrentLevel.HasValue) athlete.CurrentLevel = request.CurrentLevel.Value;
        if (request.ExperienceYears.HasValue) athlete.ExperienceYears = request.ExperienceYears.Value;
        if (request.Height is not null) athlete.Height = request.Height;
        if (request.Weight is not null) athlete.Weight = request.Weight;
        if (request.BloodGroup.HasValue) athlete.BloodGroup = request.BloodGroup.Value;
        if (request.DominantHand.HasValue) athlete.DominantHand = request.DominantHand.Value;
        if (request.DominantFoot.HasValue) athlete.DominantFoot = request.DominantFoot.Value;
        if (request.Biography is not null) athlete.Biography = request.Biography;
        if (request.Status.HasValue) athlete.Status = request.Status.Value;

        athlete.UpdatedAt = DateTime.UtcNow;
        _athleteRepository.Update(athlete);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Athlete updated: {AthleteId}", request.AthleteId);

        var dto = Commands.CreateAthlete.CreateAthleteCommandHandler.MapToDto(athlete, athlete.User);
        return Result<AthleteDto>.Success(dto);
    }
}
