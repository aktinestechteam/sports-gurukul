using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Queries.GetFacilityEquipment;

public class GetFacilityEquipmentQueryHandler : IRequestHandler<GetFacilityEquipmentQuery, Result<IReadOnlyList<EquipmentDto>>>
{
    private readonly IFacilityEquipmentRepository _equipmentRepository;
    private readonly ILogger<GetFacilityEquipmentQueryHandler> _logger;

    public GetFacilityEquipmentQueryHandler(
        IFacilityEquipmentRepository equipmentRepository,
        ILogger<GetFacilityEquipmentQueryHandler> logger)
    {
        _equipmentRepository = equipmentRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<EquipmentDto>>> Handle(GetFacilityEquipmentQuery request, CancellationToken cancellationToken)
    {
        var equipmentList = await _equipmentRepository.GetByFacilityIdAsync(request.FacilityId, cancellationToken);

        var dtos = equipmentList.Select(e => new EquipmentDto
        {
            Id = e.Id,
            FacilityId = e.FacilityId,
            EquipmentName = e.EquipmentName,
            Category = e.Category,
            PurchaseDate = e.PurchaseDate,
            Condition = e.Condition.ToString(),
            MaintenanceSchedule = e.MaintenanceSchedule,
            WarrantyExpiry = e.WarrantyExpiry,
            Quantity = e.Quantity,
            Status = e.Status.ToString(),
            Description = e.Description
        }).ToList();

        _logger.LogInformation("Retrieved {Count} equipment items for Facility: {FacilityId}", dtos.Count, request.FacilityId);

        return Result<IReadOnlyList<EquipmentDto>>.Success(dtos);
    }
}
