using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.Commands.CreateAthlete;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteByUserId;

public class GetAthleteByUserIdQueryHandler : IRequestHandler<GetAthleteByUserIdQuery, Result<AthleteDto>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly ILogger<GetAthleteByUserIdQueryHandler> _logger;

    public GetAthleteByUserIdQueryHandler(
        IAthleteRepository athleteRepository,
        ILogger<GetAthleteByUserIdQueryHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _logger = logger;
    }

    public async Task<Result<AthleteDto>> Handle(GetAthleteByUserIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching athlete by User ID: {UserId}", request.UserId);

        var athlete = await _athleteRepository.GetByUserIdWithDetailsAsync(request.UserId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete not found for user: {UserId}", request.UserId);
            return Result<AthleteDto>.Failure("Athlete profile not found for this user.");
        }

        var dto = CreateAthleteCommandHandler.MapToDto(athlete, athlete.User);
        return Result<AthleteDto>.Success(dto);
    }
}
