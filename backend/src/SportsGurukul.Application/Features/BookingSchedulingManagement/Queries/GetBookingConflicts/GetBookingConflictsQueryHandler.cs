using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingConflicts;

public class GetBookingConflictsQueryHandler : IRequestHandler<GetBookingConflictsQuery, Result<IReadOnlyList<BookingConflictDto>>>
{
    private readonly IConflictRepository _conflictRepository;
    private readonly ILogger<GetBookingConflictsQueryHandler> _logger;

    public GetBookingConflictsQueryHandler(
        IConflictRepository conflictRepository,
        ILogger<GetBookingConflictsQueryHandler> logger)
    {
        _conflictRepository = conflictRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<BookingConflictDto>>> Handle(
        GetBookingConflictsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting conflicts for booking {BookingId}", request.BookingId);

        var conflicts = await _conflictRepository.GetByBookingIdAsync(request.BookingId, cancellationToken);

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

        return Result<IReadOnlyList<BookingConflictDto>>.Success(dtos);
    }
}
