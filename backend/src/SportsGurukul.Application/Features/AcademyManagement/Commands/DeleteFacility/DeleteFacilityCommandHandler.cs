using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteFacility;

public class DeleteFacilityCommandHandler : IRequestHandler<DeleteFacilityCommand, Result<Unit>>
{
    private readonly IAcademyFacilityRepository _academyFacilityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteFacilityCommandHandler> _logger;

    public DeleteFacilityCommandHandler(
        IAcademyFacilityRepository academyFacilityRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteFacilityCommandHandler> logger)
    {
        _academyFacilityRepository = academyFacilityRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteFacilityCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting facility with Id: {FacilityId}", request.FacilityId);

        var facility = await _academyFacilityRepository.GetByIdAsync(request.FacilityId);
        if (facility is null)
            return Result<Unit>.Failure("Facility not found.");

        if (facility.IsDeleted)
            return Result<Unit>.Failure("Facility is already deleted.");

        facility.IsDeleted = true;
        facility.UpdatedAt = DateTime.UtcNow;

        _academyFacilityRepository.Update(facility);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Facility soft-deleted with Id: {FacilityId}", request.FacilityId);

        return Result<Unit>.Success(Unit.Value);
    }
}
