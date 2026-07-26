using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.UpdateFacilitySchedule;

public class UpdateFacilityScheduleCommandHandler : IRequestHandler<UpdateFacilityScheduleCommand, Result<ScheduleDto>>
{
    private readonly IFacilityScheduleRepository _scheduleRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateFacilityScheduleCommandHandler> _logger;

    public UpdateFacilityScheduleCommandHandler(
        IFacilityScheduleRepository scheduleRepository,
        IFacilityRepository facilityRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateFacilityScheduleCommandHandler> logger)
    {
        _scheduleRepository = scheduleRepository;
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ScheduleDto>> Handle(UpdateFacilityScheduleCommand request, CancellationToken cancellationToken)
    {
        var facility = await _facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);
        if (facility is null)
        {
            return Result<ScheduleDto>.Failure("Facility not found.");
        }

        var schedules = await _scheduleRepository.GetByFacilityIdAsync(request.FacilityId, cancellationToken);
        var dayOfWeek = (DayOfWeek)request.DayOfWeek;
        var existingSchedule = schedules.FirstOrDefault(s => s.DayOfWeek == dayOfWeek);

        if (existingSchedule is not null)
        {
            existingSchedule.OpeningTime = request.OpeningTime;
            existingSchedule.ClosingTime = request.ClosingTime;
            existingSchedule.IsClosed = request.IsClosed;
            existingSchedule.IsMaintenanceWindow = request.IsMaintenanceWindow;
            existingSchedule.Notes = request.Notes;
            existingSchedule.UpdatedAt = DateTime.UtcNow;

            _scheduleRepository.Update(existingSchedule);
        }
        else
        {
            var schedule = new FacilitySchedule
            {
                Id = Guid.NewGuid(),
                FacilityId = request.FacilityId,
                DayOfWeek = dayOfWeek,
                OpeningTime = request.OpeningTime,
                ClosingTime = request.ClosingTime,
                IsClosed = request.IsClosed,
                IsMaintenanceWindow = request.IsMaintenanceWindow,
                Notes = request.Notes
            };

            await _scheduleRepository.AddAsync(schedule, cancellationToken);
            existingSchedule = schedule;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Schedule updated for Facility: {FacilityId} on Day: {DayOfWeek}", request.FacilityId, dayOfWeek);

        var dto = new ScheduleDto
        {
            Id = existingSchedule.Id,
            FacilityId = existingSchedule.FacilityId,
            DayOfWeek = existingSchedule.DayOfWeek.ToString(),
            OpeningTime = existingSchedule.OpeningTime,
            ClosingTime = existingSchedule.ClosingTime,
            IsClosed = existingSchedule.IsClosed,
            IsMaintenanceWindow = existingSchedule.IsMaintenanceWindow,
            Notes = existingSchedule.Notes
        };

        return Result<ScheduleDto>.Success(dto);
    }
}
