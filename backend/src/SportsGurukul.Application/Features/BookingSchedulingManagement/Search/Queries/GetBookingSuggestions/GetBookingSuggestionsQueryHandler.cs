using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.GetBookingSuggestions;

public class GetBookingSuggestionsQueryHandler
    : IRequestHandler<GetBookingSuggestionsQuery, Result<IReadOnlyList<BookingSuggestionDto>>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<GetBookingSuggestionsQueryHandler> _logger;

    public GetBookingSuggestionsQueryHandler(
        IBookingRepository bookingRepository,
        ILogger<GetBookingSuggestionsQueryHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<BookingSuggestionDto>>> Handle(
        GetBookingSuggestionsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting booking suggestions for prefix: {Prefix}", request.Prefix);

        if (string.IsNullOrWhiteSpace(request.Prefix) || request.Prefix.Length < 2)
        {
            return Result<IReadOnlyList<BookingSuggestionDto>>.Success([]);
        }

        var bookings = await _bookingRepository.SearchAsync(
            request.AcademyId, null, null, null,
            request.Prefix, 1, request.Limit, cancellationToken);

        var suggestions = bookings.Select(b => new BookingSuggestionDto
        {
            Text = b.BookingNumber,
            Category = "Booking Number",
            RelatedId = b.Id,
            Highlight = b.Title
        }).ToList();

        var titleMatches = bookings
            .Where(b => b.Title.Contains(request.Prefix, StringComparison.OrdinalIgnoreCase))
            .Select(b => new BookingSuggestionDto
            {
                Text = b.Title,
                Category = "Title",
                RelatedId = b.Id,
                Highlight = b.BookingNumber
            });

        suggestions.AddRange(titleMatches);

        var result = suggestions
            .GroupBy(s => s.Text)
            .Select(g => g.First())
            .Take(request.Limit)
            .ToList();

        return Result<IReadOnlyList<BookingSuggestionDto>>.Success(result);
    }
}
