using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ResolveBookingConflict;

public class ResolveBookingConflictCommandHandler : IRequestHandler<ResolveBookingConflictCommand, Result<bool>>
{
    private readonly IConflictRepository _conflictRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ResolveBookingConflictCommandHandler> _logger;

    public ResolveBookingConflictCommandHandler(
        IConflictRepository conflictRepository,
        IUnitOfWork unitOfWork,
        ILogger<ResolveBookingConflictCommandHandler> logger)
    {
        _conflictRepository = conflictRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(ResolveBookingConflictCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Resolving conflict {ConflictId}", request.ConflictId);

        var conflict = await _conflictRepository.GetByIdAsync(request.ConflictId, cancellationToken);
        if (conflict is null)
            return Result<bool>.Failure("Conflict not found.");

        if (conflict.IsResolved)
            return Result<bool>.Failure("Conflict is already resolved.");

        conflict.IsResolved = true;
        conflict.ResolutionNotes = request.ResolutionNotes;
        conflict.ResolvedOn = DateTime.UtcNow;
        conflict.UpdatedAt = DateTime.UtcNow;

        _conflictRepository.Update(conflict);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Conflict {ConflictId} resolved", request.ConflictId);

        return Result<bool>.Success(true);
    }
}
