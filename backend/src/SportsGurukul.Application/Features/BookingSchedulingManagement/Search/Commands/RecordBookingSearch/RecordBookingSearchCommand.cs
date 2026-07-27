using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Commands.RecordBookingSearch;

public class RecordBookingSearchCommand : IRequest<Result<Unit>>
{
    public Guid UserId { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
    public Guid? AcademyId { get; set; }
    public Guid? FacilityId { get; set; }
    public string? BookingType { get; set; }
    public string? Status { get; set; }
    public int ResultCount { get; set; }
}
