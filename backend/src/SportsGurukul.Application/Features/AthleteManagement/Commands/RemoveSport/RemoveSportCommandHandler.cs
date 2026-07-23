using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.RemoveSport;

public class RemoveSportCommandHandler : IRequestHandler<RemoveSportCommand, Result<Unit>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly IRepository<AthleteSport> _athleteSportRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveSportCommandHandler> _logger;

    public RemoveSportCommandHandler(
        IAthleteRepository athleteRepository,
        IRepository<AthleteSport> athleteSportRepository,
        IUnitOfWork unitOfWork,
        ILogger<RemoveSportCommandHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _athleteSportRepository = athleteSportRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RemoveSportCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing sport {SportId} from athlete {AthleteId}", request.SportId, request.AthleteId);

        var athleteSports = await _athleteRepository.GetAthleteSportsAsync(request.AthleteId, cancellationToken);
        var athleteSport = athleteSports.FirstOrDefault(s => s.SportId == request.SportId && !s.IsDeleted);

        if (athleteSport is null)
        {
            _logger.LogWarning("Sport not assigned: {SportId}, {AthleteId}", request.SportId, request.AthleteId);
            return Result<Unit>.Failure("This sport is not assigned to the athlete.");
        }

        _athleteSportRepository.Remove(athleteSport);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Sport removed: {SportId}, {AthleteId}", request.SportId, request.AthleteId);
        return Result<Unit>.Success(Unit.Value);
    }
}
