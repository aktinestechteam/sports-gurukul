using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.AssignSport;

public class AssignSportCommandHandler : IRequestHandler<AssignSportCommand, Result<SportDto>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly ISportRepository _sportRepository;
    private readonly IRepository<CoachSport> _coachSportRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignSportCommandHandler> _logger;

    public AssignSportCommandHandler(
        ICoachRepository coachRepository,
        ISportRepository sportRepository,
        IRepository<CoachSport> coachSportRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignSportCommandHandler> logger)
    {
        _coachRepository = coachRepository;
        _sportRepository = sportRepository;
        _coachSportRepository = coachSportRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<SportDto>> Handle(AssignSportCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning sport {SportId} to coach {CoachId}", request.SportId, request.CoachId);

        var coach = await _coachRepository.GetByIdAsync(request.CoachId, cancellationToken);
        if (coach is null)
        {
            _logger.LogWarning("Coach not found: {CoachId}", request.CoachId);
            return Result<SportDto>.Failure("Coach not found.");
        }

        var sport = await _sportRepository.GetByIdAsync(request.SportId, cancellationToken);
        if (sport is null)
        {
            _logger.LogWarning("Sport not found: {SportId}", request.SportId);
            return Result<SportDto>.Failure("Sport not found.");
        }

        var existingSports = await _coachRepository.GetCoachSportsAsync(request.CoachId, cancellationToken);
        if (existingSports.Any(s => s.SportId == request.SportId && !s.IsDeleted))
        {
            _logger.LogWarning("Sport already assigned: {SportId}, {CoachId}", request.SportId, request.CoachId);
            return Result<SportDto>.Failure("This sport is already assigned to the coach.");
        }

        if (request.IsPrimarySport)
        {
            var currentPrimary = existingSports.FirstOrDefault(s => s.IsPrimarySport && !s.IsDeleted);
            if (currentPrimary is not null)
            {
                currentPrimary.IsPrimarySport = false;
                _coachSportRepository.Update(currentPrimary);
            }
        }

        var coachSport = new CoachSport
        {
            Id = Guid.NewGuid(),
            CoachId = request.CoachId,
            SportId = request.SportId,
            IsPrimarySport = request.IsPrimarySport,
            JoinedDate = DateTime.UtcNow
        };

        await _coachSportRepository.AddAsync(coachSport, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Sport assigned: {SportId}, {CoachId}", request.SportId, request.CoachId);

        var dto = new SportDto
        {
            Id = coachSport.Id,
            SportId = sport.Id,
            Name = sport.Name,
            Code = sport.Code,
            CategoryName = sport.SportCategory.Name,
            OlympicSport = sport.OlympicSport,
            IsPrimarySport = coachSport.IsPrimarySport,
            JoinedDate = coachSport.JoinedDate
        };

        return Result<SportDto>.Success(dto);
    }
}
