using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.AssignSport;

public class AssignSportCommandHandler : IRequestHandler<AssignSportCommand, Result<AcademySportDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly ISportRepository _sportRepository;
    private readonly IRepository<AcademySport> _academySportRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignSportCommandHandler> _logger;

    public AssignSportCommandHandler(
        IAcademyRepository academyRepository,
        ISportRepository sportRepository,
        IRepository<AcademySport> academySportRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignSportCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _sportRepository = sportRepository;
        _academySportRepository = academySportRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AcademySportDto>> Handle(AssignSportCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning sport {SportId} to academy {AcademyId}", request.SportId, request.AcademyId);

        var academy = await _academyRepository.GetByIdAsync(request.AcademyId, cancellationToken);
        if (academy is null)
        {
            _logger.LogWarning("Academy not found: {AcademyId}", request.AcademyId);
            return Result<AcademySportDto>.Failure("Academy not found.");
        }

        if (academy.VerificationStatus != VerificationStatus.Verified)
        {
            _logger.LogWarning("Academy is not verified: {AcademyId}", request.AcademyId);
            return Result<AcademySportDto>.Failure("Academy must be verified before assigning sports.");
        }

        var sport = await _sportRepository.GetByIdAsync(request.SportId, cancellationToken);
        if (sport is null)
        {
            _logger.LogWarning("Sport not found: {SportId}", request.SportId);
            return Result<AcademySportDto>.Failure("Sport not found.");
        }

        var existingSports = await _academyRepository.GetAcademySportsAsync(request.AcademyId, cancellationToken);
        if (existingSports.Any(s => s.SportId == request.SportId && !s.IsDeleted))
        {
            _logger.LogWarning("Sport already assigned: {SportId}, {AcademyId}", request.SportId, request.AcademyId);
            return Result<AcademySportDto>.Failure("This sport is already assigned to the academy.");
        }

        if (request.IsPrimarySport)
        {
            var currentPrimary = existingSports.FirstOrDefault(s => s.IsPrimarySport && !s.IsDeleted);
            if (currentPrimary is not null)
            {
                currentPrimary.IsPrimarySport = false;
                _academySportRepository.Update(currentPrimary);
            }
        }

        var academySport = new AcademySport
        {
            Id = Guid.NewGuid(),
            AcademyId = request.AcademyId,
            SportId = request.SportId,
            IsPrimarySport = request.IsPrimarySport,
            JoinedDate = DateTime.UtcNow
        };

        await _academySportRepository.AddAsync(academySport, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Sport assigned: {SportId}, {AcademyId}", request.SportId, request.AcademyId);

        var dto = new AcademySportDto
        {
            Id = academySport.Id,
            SportId = sport.Id,
            Name = sport.Name,
            Code = sport.Code,
            CategoryName = sport.SportCategory.Name,
            OlympicSport = sport.OlympicSport,
            IsPrimarySport = academySport.IsPrimarySport,
            JoinedDate = academySport.JoinedDate
        };

        return Result<AcademySportDto>.Success(dto);
    }
}
