using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RemoveFromWaitlist;

public class RemoveFromWaitlistCommandHandler : IRequestHandler<RemoveFromWaitlistCommand, Result<bool>>
{
    private readonly IWaitlistRepository _waitlistRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveFromWaitlistCommandHandler> _logger;

    public RemoveFromWaitlistCommandHandler(
        IWaitlistRepository waitlistRepository,
        IUnitOfWork unitOfWork,
        ILogger<RemoveFromWaitlistCommandHandler> logger)
    {
        _waitlistRepository = waitlistRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(RemoveFromWaitlistCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing waitlist entry {WaitlistEntryId}", request.WaitlistEntryId);

        var entry = await _waitlistRepository.GetByIdAsync(request.WaitlistEntryId, cancellationToken);
        if (entry is null)
            return Result<bool>.Failure("Waitlist entry not found.");

        entry.Status = WaitlistStatus.Cancelled;
        entry.UpdatedAt = DateTime.UtcNow;

        _waitlistRepository.Update(entry);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Waitlist entry {WaitlistEntryId} removed", request.WaitlistEntryId);

        return Result<bool>.Success(true);
    }
}
