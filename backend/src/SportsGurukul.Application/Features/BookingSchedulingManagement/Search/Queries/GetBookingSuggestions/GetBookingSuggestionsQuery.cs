using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.GetBookingSuggestions;

public class GetBookingSuggestionsQuery : IRequest<Result<IReadOnlyList<BookingSuggestionDto>>>
{
    public string Prefix { get; set; } = string.Empty;
    public Guid? AcademyId { get; set; }
    public int Limit { get; set; } = 10;
}
