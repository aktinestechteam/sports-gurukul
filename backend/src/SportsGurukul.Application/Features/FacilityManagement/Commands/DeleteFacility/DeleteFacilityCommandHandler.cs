using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.DeleteFacility;

public class DeleteFacilityCommandHandler : IRequestHandler<DeleteFacilityCommand, Result<Unit>>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteFacilityCommandHandler> _logger;

    public DeleteFacilityCommandHandler(
        IFacilityRepository facilityRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteFacilityCommandHandler> logger)
    {
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteFacilityCommand request, CancellationToken cancellationToken)
    {
        var facility = await _facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);
        if (facility is null)
        {
            return Result<Unit>.Failure("Facility not found.");
        }

        _facilityRepository.Remove(facility);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Facility soft-deleted with Id: {FacilityId}", facility.Id);

        return Result<Unit>.Success(Unit.Value);
    }
}
