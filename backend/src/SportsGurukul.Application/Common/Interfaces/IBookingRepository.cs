using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IBookingRepository : IRepository<Booking>
{
    Task<Booking?> GetByBookingNumberAsync(string bookingNumber, CancellationToken cancellationToken = default);
    Task<Booking?> GetWithDetailsAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetByAcademyIdAsync(Guid academyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetByFacilityIdAsync(Guid facilityId, DateTime date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetByCoachIdAsync(Guid coachId, DateTime date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetByAthleteIdAsync(Guid athleteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetByDateRangeAsync(Guid academyId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> SearchAsync(
        Guid? academyId,
        Guid? branchId,
        BookingType? bookingType,
        BookingStatus? status,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<int> CountSearchAsync(
        Guid? academyId,
        Guid? branchId,
        BookingType? bookingType,
        BookingStatus? status,
        string? searchTerm,
        CancellationToken cancellationToken = default);
    Task<bool> IsBookingNumberUniqueAsync(string bookingNumber, CancellationToken cancellationToken = default);
}
