using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingHistory;

public class GetBookingHistoryQueryHandler : IRequestHandler<GetBookingHistoryQuery, Result<IReadOnlyList<BookingHistoryDto>>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<GetBookingHistoryQueryHandler> _logger;

    public GetBookingHistoryQueryHandler(
        IBookingRepository bookingRepository,
        ILogger<GetBookingHistoryQueryHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<BookingHistoryDto>>> Handle(
        GetBookingHistoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting history for booking {BookingId}", request.BookingId);

        var booking = await _bookingRepository.GetWithDetailsAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result<IReadOnlyList<BookingHistoryDto>>.Failure("Booking not found.");

        var history = booking.History
            .OrderByDescending(h => h.PerformedOn)
            .Select(h => new BookingHistoryDto
            {
                Id = h.Id,
                BookingId = h.BookingId,
                Action = h.Action,
                PreviousValue = h.PreviousValue,
                NewValue = h.NewValue,
                PerformedBy = h.PerformedBy,
                PerformedOn = h.PerformedOn,
                Notes = h.Notes
            }).ToList();

        return Result<IReadOnlyList<BookingHistoryDto>>.Success(history);
    }
}
