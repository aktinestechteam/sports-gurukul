using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.CheckInParticipant;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.SearchAttendance;

public class SearchAttendanceQueryHandler : IRequestHandler<SearchAttendanceQuery, Result<PagedResult<AttendanceDto>>>
{
    private readonly IEventAttendanceRepository _attendanceRepository;
    private readonly ILogger<SearchAttendanceQueryHandler> _logger;

    public SearchAttendanceQueryHandler(
        IEventAttendanceRepository attendanceRepository,
        ILogger<SearchAttendanceQueryHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _logger = logger;
    }

    public async Task<Result<PagedResult<AttendanceDto>>> Handle(SearchAttendanceQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching attendance for event {EventId}", request.EventId);

        IReadOnlyList<Domain.Entities.EventAttendance> records;

        if (request.SessionId.HasValue)
        {
            records = await _attendanceRepository.GetBySessionIdAsync(request.SessionId.Value, cancellationToken);
        }
        else
        {
            records = await _attendanceRepository.GetByEventIdAsync(request.EventId, cancellationToken);
        }

        if (request.Status.HasValue)
        {
            records = records.Where(r => r.Status == request.Status.Value).ToList();
        }

        var totalCount = records.Count;

        var pagedRecords = records
            .OrderByDescending(r => r.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = pagedRecords.Select(r => CheckInParticipantCommandHandler.MapToDto(
            r,
            r.Participant?.ParticipantName ?? string.Empty,
            r.Session?.Title)).ToList();

        var result = new PagedResult<AttendanceDto>
        {
            Items = dtos,
            TotalRecords = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

        return Result<PagedResult<AttendanceDto>>.Success(result);
    }
}
