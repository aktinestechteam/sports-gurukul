using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.AddEquipment;

public class AddEquipmentCommandHandler : IRequestHandler<AddEquipmentCommand, Result<EquipmentDto>>
{
    private readonly IFacilityEquipmentRepository _equipmentRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddEquipmentCommandHandler> _logger;

    public AddEquipmentCommandHandler(
        IFacilityEquipmentRepository equipmentRepository,
        IFacilityRepository facilityRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddEquipmentCommandHandler> logger)
    {
        _equipmentRepository = equipmentRepository;
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EquipmentDto>> Handle(AddEquipmentCommand request, CancellationToken cancellationToken)
    {
        var facility = await _facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);
        if (facility is null)
        {
            return Result<EquipmentDto>.Failure("Facility not found.");
        }

        var equipment = new FacilityEquipment
        {
            Id = Guid.NewGuid(),
            FacilityId = request.FacilityId,
            EquipmentName = request.EquipmentName,
            Category = request.Category,
            PurchaseDate = request.PurchaseDate,
            Condition = request.Condition,
            MaintenanceSchedule = request.MaintenanceSchedule,
            WarrantyExpiry = request.WarrantyExpiry,
            Quantity = request.Quantity,
            Description = request.Description
        };

        await _equipmentRepository.AddAsync(equipment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Equipment added with Id: {EquipmentId} to Facility: {FacilityId}", equipment.Id, request.FacilityId);

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
