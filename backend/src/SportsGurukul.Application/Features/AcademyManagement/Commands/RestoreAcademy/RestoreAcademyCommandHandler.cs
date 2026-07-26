using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.RestoreAcademy;

public class RestoreAcademyCommandHandler : IRequestHandler<RestoreAcademyCommand, Result<Unit>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RestoreAcademyCommandHandler> _logger;

    public RestoreAcademyCommandHandler(
        IAcademyRepository academyRepository,
        IUnitOfWork unitOfWork,
        ILogger<RestoreAcademyCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RestoreAcademyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Restoring academy with Id: {AcademyId}", request.AcademyId);

        var academy = await _academyRepository.GetByIdAsync(request.AcademyId, cancellationToken);
        if (academy is null)
            return Result<Unit>.Failure("Academy not found.");

        if (!academy.IsDeleted)
            return Result<Unit>.Failure("Academy is not deleted.");

        academy.IsDeleted = false;
        academy.Status = AcademyStatus.Active;
        academy.UpdatedAt = DateTime.UtcNow;

        _academyRepository.Update(academy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Academy restored with Id: {AcademyId}", request.AcademyId);

        return Result<Unit>.Success(Unit.Value);
    }
}
