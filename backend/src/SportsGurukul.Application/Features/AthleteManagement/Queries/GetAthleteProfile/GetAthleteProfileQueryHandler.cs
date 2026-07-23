using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.Commands.CreateAthlete;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteProfile;

public class GetAthleteProfileQueryHandler : IRequestHandler<GetAthleteProfileQuery, Result<AthleteDto>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly ILogger<GetAthleteProfileQueryHandler> _logger;

    public GetAthleteProfileQueryHandler(
        IAthleteRepository athleteRepository,
        ILogger<GetAthleteProfileQueryHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _logger = logger;
    }

    public async Task<Result<AthleteDto>> Handle(GetAthleteProfileQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching athlete profile: {AthleteId}", request.AthleteId);

        var athlete = await _athleteRepository.GetByIdWithDetailsAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete profile not found: {AthleteId}", request.AthleteId);
            return Result<AthleteDto>.Failure("Athlete profile not found.");
        }

        var dto = CreateAthleteCommandHandler.MapToDto(athlete, athlete.User);
        return Result<AthleteDto>.Success(dto);
    }
}
