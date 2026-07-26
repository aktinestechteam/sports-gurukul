using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.RemoveSport;

public class RemoveSportCommandHandler : IRequestHandler<RemoveSportCommand, Result<Unit>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IRepository<AcademySport> _academySportRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveSportCommandHandler> _logger;

    public RemoveSportCommandHandler(
        IAcademyRepository academyRepository,
        IRepository<AcademySport> academySportRepository,
        IUnitOfWork unitOfWork,
        ILogger<RemoveSportCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _academySportRepository = academySportRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RemoveSportCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing sport {SportId} from academy {AcademyId}", request.SportId, request.AcademyId);

        var academy = await _academyRepository.GetByIdAsync(request.AcademyId, cancellationToken);
        if (academy is null)
            return Result<Unit>.Failure("Academy not found.");

        var academySports = await _academyRepository.GetAcademySportsAsync(request.AcademyId, cancellationToken);
        var academySport = academySports.FirstOrDefault(s => s.SportId == request.SportId && !s.IsDeleted);

        if (academySport is null)
        {
            _logger.LogWarning("Sport not assigned: {SportId}, {AcademyId}", request.SportId, request.AcademyId);
            return Result<Unit>.Failure("This sport is not assigned to the academy.");
        }

        _academySportRepository.Remove(academySport);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Sport removed: {SportId}, {AcademyId}", request.SportId, request.AcademyId);
        return Result<Unit>.Success(Unit.Value);
    }
}
