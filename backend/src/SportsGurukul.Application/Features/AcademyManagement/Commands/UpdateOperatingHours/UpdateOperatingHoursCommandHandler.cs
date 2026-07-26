using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateOperatingHours;

public class UpdateOperatingHoursCommandHandler : IRequestHandler<UpdateOperatingHoursCommand, Result<OperatingHoursDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateOperatingHoursCommandHandler> _logger;

    public UpdateOperatingHoursCommandHandler(
        IAcademyRepository academyRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateOperatingHoursCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<OperatingHoursDto>> Handle(UpdateOperatingHoursCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating operating hours for academy: {AcademyId}", request.AcademyId);

        var academy = await _academyRepository.GetByIdWithDetailsAsync(request.AcademyId, cancellationToken);
        if (academy is null)
            return Result<OperatingHoursDto>.Failure("Academy not found.");

        var operatingHours = academy.OperatingHours;

        if (operatingHours is null)
        {
            operatingHours = new AcademyOperatingHours
            {
                Id = Guid.NewGuid(),
                AcademyId = request.AcademyId,
                MondayOpening = ParseTime(request.MondayOpening),
                MondayClosing = ParseTime(request.MondayClosing),
                TuesdayOpening = ParseTime(request.TuesdayOpening),
                TuesdayClosing = ParseTime(request.TuesdayClosing),
                WednesdayOpening = ParseTime(request.WednesdayOpening),
                WednesdayClosing = ParseTime(request.WednesdayClosing),
                ThursdayOpening = ParseTime(request.ThursdayOpening),
                ThursdayClosing = ParseTime(request.ThursdayClosing),
                FridayOpening = ParseTime(request.FridayOpening),
                FridayClosing = ParseTime(request.FridayClosing),
                SaturdayOpening = ParseTime(request.SaturdayOpening),
                SaturdayClosing = ParseTime(request.SaturdayClosing),
                SundayOpening = ParseTime(request.SundayOpening),
                SundayClosing = ParseTime(request.SundayClosing),
                HolidaySchedule = request.HolidaySchedule
            };

            academy.OperatingHours = operatingHours;
        }
        else
        {
            if (request.MondayOpening is not null)
                operatingHours.MondayOpening = ParseTime(request.MondayOpening);

            if (request.MondayClosing is not null)
                operatingHours.MondayClosing = ParseTime(request.MondayClosing);

            if (request.TuesdayOpening is not null)
                operatingHours.TuesdayOpening = ParseTime(request.TuesdayOpening);

            if (request.TuesdayClosing is not null)
                operatingHours.TuesdayClosing = ParseTime(request.TuesdayClosing);

            if (request.WednesdayOpening is not null)
                operatingHours.WednesdayOpening = ParseTime(request.WednesdayOpening);

            if (request.WednesdayClosing is not null)
                operatingHours.WednesdayClosing = ParseTime(request.WednesdayClosing);

            if (request.ThursdayOpening is not null)
                operatingHours.ThursdayOpening = ParseTime(request.ThursdayOpening);

            if (request.ThursdayClosing is not null)
                operatingHours.ThursdayClosing = ParseTime(request.ThursdayClosing);

            if (request.FridayOpening is not null)
                operatingHours.FridayOpening = ParseTime(request.FridayOpening);

            if (request.FridayClosing is not null)
                operatingHours.FridayClosing = ParseTime(request.FridayClosing);

            if (request.SaturdayOpening is not null)
                operatingHours.SaturdayOpening = ParseTime(request.SaturdayOpening);

            if (request.SaturdayClosing is not null)
                operatingHours.SaturdayClosing = ParseTime(request.SaturdayClosing);

            if (request.SundayOpening is not null)
                operatingHours.SundayOpening = ParseTime(request.SundayOpening);

            if (request.SundayClosing is not null)
                operatingHours.SundayClosing = ParseTime(request.SundayClosing);

            if (request.HolidaySchedule is not null)
                operatingHours.HolidaySchedule = request.HolidaySchedule;

            operatingHours.UpdatedAt = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Operating hours updated for academy: {AcademyId}", request.AcademyId);

        var dto = new OperatingHoursDto
        {
            Id = operatingHours.Id,
            AcademyId = operatingHours.AcademyId,
            MondayOpening = operatingHours.MondayOpening?.ToString("HH:mm"),
            MondayClosing = operatingHours.MondayClosing?.ToString("HH:mm"),
            TuesdayOpening = operatingHours.TuesdayOpening?.ToString("HH:mm"),
            TuesdayClosing = operatingHours.TuesdayClosing?.ToString("HH:mm"),
            WednesdayOpening = operatingHours.WednesdayOpening?.ToString("HH:mm"),
            WednesdayClosing = operatingHours.WednesdayClosing?.ToString("HH:mm"),
            ThursdayOpening = operatingHours.ThursdayOpening?.ToString("HH:mm"),
            ThursdayClosing = operatingHours.ThursdayClosing?.ToString("HH:mm"),
            FridayOpening = operatingHours.FridayOpening?.ToString("HH:mm"),
            FridayClosing = operatingHours.FridayClosing?.ToString("HH:mm"),
            SaturdayOpening = operatingHours.SaturdayOpening?.ToString("HH:mm"),
            SaturdayClosing = operatingHours.SaturdayClosing?.ToString("HH:mm"),
            SundayOpening = operatingHours.SundayOpening?.ToString("HH:mm"),
            SundayClosing = operatingHours.SundayClosing?.ToString("HH:mm"),
            HolidaySchedule = operatingHours.HolidaySchedule,
            CreatedAt = operatingHours.CreatedAt,
            UpdatedAt = operatingHours.UpdatedAt
        };

        return Result<OperatingHoursDto>.Success(dto);
    }

    private static TimeOnly? ParseTime(string? timeString)
    {
        if (string.IsNullOrWhiteSpace(timeString))
            return null;

        if (TimeOnly.TryParse(timeString, out var time))
            return time;

        return null;
    }
}
