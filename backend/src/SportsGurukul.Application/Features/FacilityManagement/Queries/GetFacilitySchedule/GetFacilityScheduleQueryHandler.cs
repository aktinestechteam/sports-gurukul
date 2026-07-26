using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Queries.GetFacilitySchedule;

public class GetFacilityScheduleQueryHandler : IRequestHandler<GetFacilityScheduleQuery, Result<IReadOnlyList<ScheduleDto>>>
{
    private readonly IFacilityScheduleRepository _scheduleRepository;
    private readonly ILogger<GetFacilityScheduleQueryHandler> _logger;

    public GetFacilityScheduleQueryHandler(
        IFacilityScheduleRepository scheduleRepository,
        ILogger<GetFacilityScheduleQueryHandler> logger)
    {
        _scheduleRepository = scheduleRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<ScheduleDto>>> Handle(GetFacilityScheduleQuery request, CancellationToken cancellationToken)
    {
        var schedules = await _scheduleRepository.GetByFacilityIdAsync(request.FacilityId, cancellationToken);

        var dtos = schedules.Select(s => new ScheduleDto
        {
            Id = s.Id,
            FacilityId = s.FacilityId,
            DayOfWeek = s.DayOfWeek.ToString(),
            OpeningTime = s.OpeningTime,
            ClosingTime = s.ClosingTime,
            IsClosed = s.IsClosed,
            IsMaintenanceWindow = s.IsMaintenanceWindow,
            Notes = s.Notes
        }).ToList();

        _logger.LogInformation("Retrieved {Count} schedule entries for Facility: {FacilityId}", dtos.Count, request.FacilityId);

        return Result<IReadOnlyList<ScheduleDto>>.Success(dtos);
    }
}
