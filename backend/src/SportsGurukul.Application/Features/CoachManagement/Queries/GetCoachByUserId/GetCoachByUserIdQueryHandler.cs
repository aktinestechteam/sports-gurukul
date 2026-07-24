using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.Commands.CreateCoach;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachByUserId;

public class GetCoachByUserIdQueryHandler : IRequestHandler<GetCoachByUserIdQuery, Result<CoachDto>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly ILogger<GetCoachByUserIdQueryHandler> _logger;

    public GetCoachByUserIdQueryHandler(
        ICoachRepository coachRepository,
        ILogger<GetCoachByUserIdQueryHandler> logger)
    {
        _coachRepository = coachRepository;
        _logger = logger;
    }

    public async Task<Result<CoachDto>> Handle(GetCoachByUserIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting coach for UserId: {UserId}", request.UserId);

        var coach = await _coachRepository.GetByUserIdWithDetailsAsync(request.UserId, cancellationToken);
        if (coach is null)
            return Result<CoachDto>.Failure("Coach not found for the given user.");

        return Result<CoachDto>.Success(CreateCoachCommandHandler.MapToDto(coach, coach.User));
    }
}
