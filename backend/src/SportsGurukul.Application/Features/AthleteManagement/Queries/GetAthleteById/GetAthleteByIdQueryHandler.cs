using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.Commands.CreateAthlete;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteById;

public class GetAthleteByIdQueryHandler : IRequestHandler<GetAthleteByIdQuery, Result<AthleteDto>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly ILogger<GetAthleteByIdQueryHandler> _logger;

    public GetAthleteByIdQueryHandler(
        IAthleteRepository athleteRepository,
        ILogger<GetAthleteByIdQueryHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _logger = logger;
    }

    public async Task<Result<AthleteDto>> Handle(GetAthleteByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching athlete by ID: {AthleteId}", request.AthleteId);

        var athlete = await _athleteRepository.GetByIdWithDetailsAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete not found: {AthleteId}", request.AthleteId);
            return Result<AthleteDto>.Failure("Athlete not found.");
        }

        var dto = CreateAthleteCommandHandler.MapToDto(athlete, athlete.User);
        return Result<AthleteDto>.Success(dto);
    }
}
