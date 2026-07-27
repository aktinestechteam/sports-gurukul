using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.SearchBookings;

public class SearchBookingsQuery : IRequest<Result<(IReadOnlyList<BookingSummaryDto> Items, int TotalCount)>>
{
    public Guid? AcademyId { get; set; }
    public Guid? BranchId { get; set; }
    public string? BookingType { get; set; }
    public string? Status { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
