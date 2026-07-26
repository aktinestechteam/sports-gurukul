using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.ScheduleMaintenance;

public class ScheduleMaintenanceCommandHandler : IRequestHandler<ScheduleMaintenanceCommand, Result<MaintenanceDto>>
{
    private readonly IFacilityEquipmentRepository _equipmentRepository;
    private readonly IRepository<EquipmentMaintenance> _maintenanceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ScheduleMaintenanceCommandHandler> _logger;

    public ScheduleMaintenanceCommandHandler(
        IFacilityEquipmentRepository equipmentRepository,
        IRepository<EquipmentMaintenance> maintenanceRepository,
        IUnitOfWork unitOfWork,
        ILogger<ScheduleMaintenanceCommandHandler> logger)
    {
        _equipmentRepository = equipmentRepository;
        _maintenanceRepository = maintenanceRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<MaintenanceDto>> Handle(ScheduleMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(request.EquipmentId, cancellationToken);
        if (equipment is null)
        {
            return Result<MaintenanceDto>.Failure("Equipment not found.");
        }

        var maintenance = new EquipmentMaintenance
        {
            Id = Guid.NewGuid(),
            FacilityEquipmentId = request.EquipmentId,
            ScheduledDate = request.ScheduledDate,
            MaintenanceType = request.MaintenanceType,
            Description = request.Description,
            Cost = request.Cost,
            PerformedBy = request.PerformedBy,
            Notes = request.Notes,
            IsCompleted = false
        };

        await _maintenanceRepository.AddAsync(maintenance, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Maintenance scheduled with Id: {MaintenanceId} for Equipment: {EquipmentId}", maintenance.Id, request.EquipmentId);

        var dto = new MaintenanceDto
        {
            Id = maintenance.Id,
            FacilityEquipmentId = maintenance.FacilityEquipmentId,
            ScheduledDate = maintenance.ScheduledDate,
            CompletedDate = maintenance.CompletedDate,
            MaintenanceType = maintenance.MaintenanceType,
            Description = maintenance.Description,
            Cost = maintenance.Cost,
            PerformedBy = maintenance.PerformedBy,
            Notes = maintenance.Notes,
            IsCompleted = maintenance.IsCompleted
        };

        return Result<MaintenanceDto>.Success(dto);
    }
}
