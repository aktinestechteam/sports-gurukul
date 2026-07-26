using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.DeleteCourt;

public class DeleteCourtCommandHandler : IRequestHandler<DeleteCourtCommand, Result<Unit>>
{
    private readonly IFacilityCourtRepository _courtRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCourtCommandHandler> _logger;

    public DeleteCourtCommandHandler(
        IFacilityCourtRepository courtRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteCourtCommandHandler> logger)
    {
        _courtRepository = courtRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteCourtCommand request, CancellationToken cancellationToken)
    {
        var court = await _courtRepository.GetByIdAsync(request.CourtId, cancellationToken);
        if (court is null)
        {
            return Result<Unit>.Failure("Court not found.");
        }

        _courtRepository.Remove(court);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Court soft-deleted with Id: {CourtId}", court.Id);

        return Result<Unit>.Success(Unit.Value);
    }
}
