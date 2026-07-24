using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.Commands.CreateCoach;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachById;

public class GetCoachByIdQueryHandler : IRequestHandler<GetCoachByIdQuery, Result<CoachDto>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly ILogger<GetCoachByIdQueryHandler> _logger;

    public GetCoachByIdQueryHandler(
        ICoachRepository coachRepository,
        ILogger<GetCoachByIdQueryHandler> logger)
    {
        _coachRepository = coachRepository;
        _logger = logger;
    }

    public async Task<Result<CoachDto>> Handle(GetCoachByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting coach with Id: {CoachId}", request.CoachId);

        var coach = await _coachRepository.GetByIdWithDetailsAsync(request.CoachId, cancellationToken);
        if (coach is null)
            return Result<CoachDto>.Failure("Coach not found.");

        return Result<CoachDto>.Success(CreateCoachCommandHandler.MapToDto(coach, coach.User));
    }
}
