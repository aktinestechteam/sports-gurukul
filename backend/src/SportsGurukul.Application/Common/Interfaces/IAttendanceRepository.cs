using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IAttendanceRepository : IRepository<Attendance>
{
    Task<IReadOnlyList<Attendance>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Attendance>> GetByAthleteIdAsync(Guid athleteId, CancellationToken cancellationToken = default);
    Task<Attendance?> GetBySessionAndAthleteAsync(Guid sessionId, Guid athleteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Attendance>> GetBySessionIdWithDetailsAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Attendance>> GetByStatusAsync(AttendanceStatus status, CancellationToken cancellationToken = default);
}
