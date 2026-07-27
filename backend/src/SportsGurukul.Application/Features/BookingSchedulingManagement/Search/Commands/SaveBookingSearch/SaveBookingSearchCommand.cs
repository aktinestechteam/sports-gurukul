using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Commands.SaveBookingSearch;

public class SaveBookingSearchCommand : IRequest<Result<SavedBookingSearchDto>>
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
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
}
