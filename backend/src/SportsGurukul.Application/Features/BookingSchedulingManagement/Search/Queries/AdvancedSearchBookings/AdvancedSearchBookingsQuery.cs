using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.AdvancedSearchBookings;

public class AdvancedSearchBookingsQuery : IRequest<Result<BookingSearchPageResultDto>>
{
    public string? SearchTerm { get; set; }
    public string? BookingNumber { get; set; }
    public string? Title { get; set; }
    public Guid? AcademyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? FacilityId { get; set; }
    public Guid? CoachId { get; set; }
    public Guid? AthleteId { get; set; }
    public string? BookingType { get; set; }
    public string? Status { get; set; }
    public string? ApprovalStatus { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public TimeSpan? StartTimeFrom { get; set; }
    public TimeSpan? StartTimeTo { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
