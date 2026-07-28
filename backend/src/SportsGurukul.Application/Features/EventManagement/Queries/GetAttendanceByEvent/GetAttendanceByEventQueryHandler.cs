using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Queries.GetAttendanceByEvent;

public class GetAttendanceByEventQueryHandler : IRequestHandler<GetAttendanceByEventQuery, Result<PagedResult<AttendanceDto>>>
{
    private readonly IEventAttendanceRepository _attendanceRepository;
    private readonly ILogger<GetAttendanceByEventQueryHandler> _logger;

    public GetAttendanceByEventQueryHandler(
        IEventAttendanceRepository attendanceRepository,
        ILogger<GetAttendanceByEventQueryHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _logger = logger;
    }

    public async Task<Result<PagedResult<AttendanceDto>>> Handle(GetAttendanceByEventQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting attendance for event: {EventId}, SessionId={SessionId}, Status={Status}, Page={Page}, PageSize={PageSize}",
            request.EventId, request.SessionId, request.Status, request.Page, request.PageSize);

        IReadOnlyList<Domain.Entities.EventAttendance> attendanceRecords;

        if (request.SessionId.HasValue)
        {
            attendanceRecords = await _attendanceRepository.GetBySessionIdAsync(request.SessionId.Value, cancellationToken);
        }
        else
        {
            attendanceRecords = await _attendanceRepository.GetByEventIdAsync(request.EventId, cancellationToken);
        }

        if (request.Status.HasValue)
        {
            attendanceRecords = attendanceRecords.Where(a => a.Status == request.Status.Value).ToList();
        }

        var totalCount = attendanceRecords.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var pagedItems = attendanceRecords
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var items = pagedItems.Select(a => new AttendanceDto
        {
            Id = a.Id,
            EventId = a.EventId,
            SessionId = a.SessionId,
            ParticipantId = a.ParticipantId,
            Status = a.Status.ToString(),
            CheckInTime = a.CheckInTime,
            CheckOutTime = a.CheckOutTime,
            Remarks = a.Remarks,
            MarkedBy = a.MarkedBy,
            CreatedAt = a.CreatedAt
        }).ToList();

        var result = new PagedResult<AttendanceDto>
        {
            Items = items,
            TotalRecords = totalCount,
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

        return Result<PagedResult<AttendanceDto>>.Success(result);
    }
}
