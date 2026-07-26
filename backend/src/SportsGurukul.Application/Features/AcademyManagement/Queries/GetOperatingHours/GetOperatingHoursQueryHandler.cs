using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetOperatingHours;

public class GetOperatingHoursQueryHandler : IRequestHandler<GetOperatingHoursQuery, Result<OperatingHoursDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly ILogger<GetOperatingHoursQueryHandler> _logger;

    public GetOperatingHoursQueryHandler(
        IAcademyRepository academyRepository,
        ILogger<GetOperatingHoursQueryHandler> logger)
    {
        _academyRepository = academyRepository;
        _logger = logger;
    }

    public async Task<Result<OperatingHoursDto>> Handle(GetOperatingHoursQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting operating hours for academy: {AcademyId}", request.AcademyId);

        var academy = await _academyRepository.GetByIdWithDetailsAsync(request.AcademyId, cancellationToken);
        if (academy is null)
            return Result<OperatingHoursDto>.Failure("Academy not found.");

        if (academy.OperatingHours is null)
            return Result<OperatingHoursDto>.Failure("Operating hours not configured for this academy.");

        var oh = academy.OperatingHours;
        var dto = new OperatingHoursDto
        {
            Id = oh.Id,
            AcademyId = oh.AcademyId,
            MondayOpening = oh.MondayOpening?.ToString("HH:mm"),
            MondayClosing = oh.MondayClosing?.ToString("HH:mm"),
            TuesdayOpening = oh.TuesdayOpening?.ToString("HH:mm"),
            TuesdayClosing = oh.TuesdayClosing?.ToString("HH:mm"),
            WednesdayOpening = oh.WednesdayOpening?.ToString("HH:mm"),
            WednesdayClosing = oh.WednesdayClosing?.ToString("HH:mm"),
            ThursdayOpening = oh.ThursdayOpening?.ToString("HH:mm"),
            ThursdayClosing = oh.ThursdayClosing?.ToString("HH:mm"),
            FridayOpening = oh.FridayOpening?.ToString("HH:mm"),
            FridayClosing = oh.FridayClosing?.ToString("HH:mm"),
            SaturdayOpening = oh.SaturdayOpening?.ToString("HH:mm"),
            SaturdayClosing = oh.SaturdayClosing?.ToString("HH:mm"),
            SundayOpening = oh.SundayOpening?.ToString("HH:mm"),
            SundayClosing = oh.SundayClosing?.ToString("HH:mm"),
            HolidaySchedule = oh.HolidaySchedule,
            CreatedAt = oh.CreatedAt,
            UpdatedAt = oh.UpdatedAt
        };

        return Result<OperatingHoursDto>.Success(dto);
    }
}
