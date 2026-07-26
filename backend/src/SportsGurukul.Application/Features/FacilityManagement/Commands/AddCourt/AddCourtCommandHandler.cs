using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.AddCourt;

public class AddCourtCommandHandler : IRequestHandler<AddCourtCommand, Result<CourtDto>>
{
    private readonly IFacilityCourtRepository _courtRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddCourtCommandHandler> _logger;

    public AddCourtCommandHandler(
        IFacilityCourtRepository courtRepository,
        IFacilityRepository facilityRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddCourtCommandHandler> logger)
    {
        _courtRepository = courtRepository;
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CourtDto>> Handle(AddCourtCommand request, CancellationToken cancellationToken)
    {
        var facility = await _facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);
        if (facility is null)
        {
            return Result<CourtDto>.Failure("Facility not found.");
        }

        var isCourtNumberUnique = await _courtRepository.IsCourtNumberUniqueInFacilityAsync(
            request.FacilityId, request.CourtNumber, cancellationToken);

        if (!isCourtNumberUnique)
        {
            return Result<CourtDto>.Failure("A court with this number already exists in the facility.");
        }

        var court = new FacilityCourt
        {
            Id = Guid.NewGuid(),
            FacilityId = request.FacilityId,
            CourtNumber = request.CourtNumber,
            CourtName = request.CourtName,
            CourtType = request.CourtType,
            Capacity = request.Capacity,
            Description = request.Description
        };

        await _courtRepository.AddAsync(court, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Court added with Id: {CourtId} to Facility: {FacilityId}", court.Id, request.FacilityId);

        var dto = new CourtDto
        {
            Id = court.Id,
            FacilityId = court.FacilityId,
            CourtNumber = court.CourtNumber,
            CourtName = court.CourtName,
            CourtType = court.CourtType,
            Capacity = court.Capacity,
            Status = court.Status.ToString(),
            Description = court.Description
        };

        return Result<CourtDto>.Success(dto);
    }
}
