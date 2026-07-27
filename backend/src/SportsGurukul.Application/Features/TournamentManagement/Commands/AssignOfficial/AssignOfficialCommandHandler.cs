using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.AssignOfficial;

public class AssignOfficialCommandHandler : IRequestHandler<AssignOfficialCommand, Result<Unit>>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignOfficialCommandHandler> _logger;

    public AssignOfficialCommandHandler(
        ITournamentRepository tournamentRepository,
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        ILogger<AssignOfficialCommandHandler> logger)
    {
        _tournamentRepository = tournamentRepository;
        _context = context;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(AssignOfficialCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning official to tournament: {TournamentId}", request.TournamentId);

        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId, cancellationToken);
        if (tournament is null)
            return Result<Unit>.Failure("Tournament not found.");

        var official = new TournamentOfficial
        {
            Id = Guid.NewGuid(),
            TournamentId = request.TournamentId,
            CoachId = request.CoachId,
            OfficialName = request.OfficialName,
            Role = request.Role,
            Email = request.Email,
            Phone = request.Phone,
            IsActive = true
        };

        _context.TournamentOfficials.Add(official);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Official assigned: {OfficialId} to tournament: {TournamentId}", official.Id, request.TournamentId);
        return Result<Unit>.Success(Unit.Value);
    }
}
