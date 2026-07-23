using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.AssignSport;

public class AssignSportCommandHandler : IRequestHandler<AssignSportCommand, Result<SportDto>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly ISportRepository _sportRepository;
    private readonly IRepository<AthleteSport> _athleteSportRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignSportCommandHandler> _logger;

    public AssignSportCommandHandler(
        IAthleteRepository athleteRepository,
        ISportRepository sportRepository,
        IRepository<AthleteSport> athleteSportRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignSportCommandHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _sportRepository = sportRepository;
        _athleteSportRepository = athleteSportRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<SportDto>> Handle(AssignSportCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning sport {SportId} to athlete {AthleteId}", request.SportId, request.AthleteId);

        var athlete = await _athleteRepository.GetByIdAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete not found: {AthleteId}", request.AthleteId);
            return Result<SportDto>.Failure("Athlete not found.");
        }

        var sport = await _sportRepository.GetByIdAsync(request.SportId, cancellationToken);
        if (sport is null)
        {
            _logger.LogWarning("Sport not found: {SportId}", request.SportId);
            return Result<SportDto>.Failure("Sport not found.");
        }

        var existingSports = await _athleteRepository.GetAthleteSportsAsync(request.AthleteId, cancellationToken);
        if (existingSports.Any(s => s.SportId == request.SportId && !s.IsDeleted))
        {
            _logger.LogWarning("Sport already assigned: {SportId}, {AthleteId}", request.SportId, request.AthleteId);
            return Result<SportDto>.Failure("This sport is already assigned to the athlete.");
        }

        if (request.IsPrimarySport)
        {
            var currentPrimary = existingSports.FirstOrDefault(s => s.IsPrimarySport && !s.IsDeleted);
            if (currentPrimary is not null)
            {
                currentPrimary.IsPrimarySport = false;
                _athleteSportRepository.Update(currentPrimary);
            }
        }

        var athleteSport = new AthleteSport
        {
            Id = Guid.NewGuid(),
            AthleteId = request.AthleteId,
            SportId = request.SportId,
            IsPrimarySport = request.IsPrimarySport,
            JoinedDate = DateTime.UtcNow
        };

        await _athleteSportRepository.AddAsync(athleteSport, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Sport assigned: {SportId}, {AthleteId}", request.SportId, request.AthleteId);

        var dto = new SportDto
        {
            Id = athleteSport.Id,
            SportId = sport.Id,
            Name = sport.Name,
            Code = sport.Code,
            CategoryName = sport.SportCategory.Name,
            OlympicSport = sport.OlympicSport,
            IsPrimarySport = athleteSport.IsPrimarySport,
            JoinedDate = athleteSport.JoinedDate
        };

        return Result<SportDto>.Success(dto);
    }
}
