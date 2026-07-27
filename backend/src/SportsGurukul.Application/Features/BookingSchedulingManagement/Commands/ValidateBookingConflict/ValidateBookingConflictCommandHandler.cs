using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Services;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ValidateBookingConflict;

public class ValidateBookingConflictCommandHandler : IRequestHandler<ValidateBookingConflictCommand, Result<IReadOnlyList<BookingConflictDto>>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IConflictDetectionService _conflictDetectionService;
    private readonly ILogger<ValidateBookingConflictCommandHandler> _logger;

    public ValidateBookingConflictCommandHandler(
        IBookingRepository bookingRepository,
        IConflictDetectionService conflictDetectionService,
        ILogger<ValidateBookingConflictCommandHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _conflictDetectionService = conflictDetectionService;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<BookingConflictDto>>> Handle(ValidateBookingConflictCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Validating conflicts for booking {BookingId}", request.BookingId);

        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result<IReadOnlyList<BookingConflictDto>>.Failure("Booking not found.");

        var conflicts = await _conflictDetectionService.DetectConflictsAsync(booking, cancellationToken);

        var dtos = conflicts.Select(c => new BookingConflictDto
        {
            Id = c.Id,
            BookingId = c.BookingId,
            ConflictingBookingId = c.ConflictingBookingId,
            ConflictType = c.ConflictType.ToString(),
            Description = c.Description,
            IsResolved = c.IsResolved,
            ResolutionNotes = c.ResolutionNotes,
            ResolvedOn = c.ResolvedOn,
            CreatedAt = c.CreatedAt
        }).ToList();

        _logger.LogInformation("Found {Count} conflicts for booking {BookingNumber}", dtos.Count, booking.BookingNumber);

        return Result<IReadOnlyList<BookingConflictDto>>.Success(dtos);
    }
}
