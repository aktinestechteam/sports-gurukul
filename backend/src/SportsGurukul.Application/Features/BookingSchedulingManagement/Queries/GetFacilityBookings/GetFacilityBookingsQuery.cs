using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetFacilityBookings;

public class GetFacilityBookingsQuery : IRequest<Result<IReadOnlyList<BookingSummaryDto>>>
{
    public Guid FacilityId { get; set; }
    public DateTime Date { get; set; }
}
