using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.UpdateEquipment;

public class UpdateEquipmentCommandHandler : IRequestHandler<UpdateEquipmentCommand, Result<EquipmentDto>>
{
    private readonly IFacilityEquipmentRepository _equipmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateEquipmentCommandHandler> _logger;

    public UpdateEquipmentCommandHandler(
        IFacilityEquipmentRepository equipmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateEquipmentCommandHandler> logger)
    {
        _equipmentRepository = equipmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EquipmentDto>> Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(request.EquipmentId, cancellationToken);
        if (equipment is null)
        {
            return Result<EquipmentDto>.Failure("Equipment not found.");
        }

        if (request.EquipmentName is not null)
            equipment.EquipmentName = request.EquipmentName;
        if (request.Category is not null)
            equipment.Category = request.Category;
        if (request.Condition is not null)
            equipment.Condition = request.Condition.Value;
        if (request.MaintenanceSchedule is not null)
            equipment.MaintenanceSchedule = request.MaintenanceSchedule;
        if (request.WarrantyExpiry is not null)
            equipment.WarrantyExpiry = request.WarrantyExpiry;
        if (request.Quantity is not null)
            equipment.Quantity = request.Quantity.Value;
        if (request.Status is not null)
            equipment.Status = request.Status.Value;
        if (request.Description is not null)
            equipment.Description = request.Description;

        equipment.UpdatedAt = DateTime.UtcNow;

        _equipmentRepository.Update(equipment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Equipment updated with Id: {EquipmentId}", equipment.Id);

        var dto = new EquipmentDto
        {
            Id = equipment.Id,
            FacilityId = equipment.FacilityId,
            EquipmentName = equipment.EquipmentName,
            Category = equipment.Category,
            PurchaseDate = equipment.PurchaseDate,
            Condition = equipment.Condition.ToString(),
            MaintenanceSchedule = equipment.MaintenanceSchedule,
            WarrantyExpiry = equipment.WarrantyExpiry,
            Quantity = equipment.Quantity,
            Status = equipment.Status.ToString(),
            Description = equipment.Description
        };

        return Result<EquipmentDto>.Success(dto);
    }
}
