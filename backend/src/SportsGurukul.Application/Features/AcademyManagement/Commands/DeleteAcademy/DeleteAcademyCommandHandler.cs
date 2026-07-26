using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteAcademy;

public class DeleteAcademyCommandHandler : IRequestHandler<DeleteAcademyCommand, Result<Unit>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteAcademyCommandHandler> _logger;

    public DeleteAcademyCommandHandler(
        IAcademyRepository academyRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteAcademyCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteAcademyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting academy with Id: {AcademyId}", request.AcademyId);

        var academy = await _academyRepository.GetByIdAsync(request.AcademyId, cancellationToken);
        if (academy is null)
            return Result<Unit>.Failure("Academy not found.");

        if (academy.IsDeleted)
            return Result<Unit>.Failure("Academy is already deleted.");

        academy.IsDeleted = true;
        academy.Status = AcademyStatus.Inactive;
        academy.UpdatedAt = DateTime.UtcNow;

        _academyRepository.Update(academy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Academy soft-deleted with Id: {AcademyId}", request.AcademyId);

        return Result<Unit>.Success(Unit.Value);
    }
}
