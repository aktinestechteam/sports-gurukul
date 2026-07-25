using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.Commands.CreateCoach;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeactivateCoach;

public class DeactivateCoachCommandHandler : IRequestHandler<DeactivateCoachCommand, Result<CoachDto>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeactivateCoachCommandHandler> _logger;

    public DeactivateCoachCommandHandler(
        ICoachRepository coachRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeactivateCoachCommandHandler> logger)
    {
        _coachRepository = coachRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CoachDto>> Handle(DeactivateCoachCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deactivating coach with Id: {CoachId}", request.CoachId);

        var coach = await _coachRepository.GetByIdWithDetailsAsync(request.CoachId, cancellationToken);
        if (coach is null)
            return Result<CoachDto>.Failure("Coach not found.");

        coach.Status = CoachStatus.Inactive;
        coach.UpdatedAt = DateTime.UtcNow;

        _coachRepository.Update(coach);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Coach deactivated with Id: {CoachId}", request.CoachId);

        return Result<CoachDto>.Success(CreateCoachCommandHandler.MapToDto(coach, coach.User));
    }
}
