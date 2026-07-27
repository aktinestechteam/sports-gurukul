using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Application.Features.TournamentManagement.Queries.SearchTournaments;

public class SearchTournamentsQueryHandler : IRequestHandler<SearchTournamentsQuery, Result<TournamentSearchResponse>>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly ILogger<SearchTournamentsQueryHandler> _logger;

    public SearchTournamentsQueryHandler(
        ITournamentRepository tournamentRepository,
        ILogger<SearchTournamentsQueryHandler> logger)
    {
        _tournamentRepository = tournamentRepository;
        _logger = logger;
    }

    public async Task<Result<TournamentSearchResponse>> Handle(SearchTournamentsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching tournaments: Page={Page}, PageSize={PageSize}", request.Page, request.PageSize);

        var tournaments = await _tournamentRepository.SearchAsync(
            request.AcademyId, request.Status, request.TournamentType, request.SearchTerm,
            request.Page, request.PageSize, cancellationToken);

        var totalCount = await _tournamentRepository.CountSearchAsync(
            request.AcademyId, request.Status, request.TournamentType, request.SearchTerm, cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var items = tournaments.Select(t => new TournamentSummaryDto
        {
            Id = t.Id,
            TournamentCode = t.TournamentCode,
            TournamentName = t.TournamentName,
            Description = t.Description,
            TournamentType = t.TournamentType,
            Status = t.Status,
            StartDate = t.StartDate,
            EndDate = t.EndDate,
            MaxParticipants = t.MaxParticipants,
            RegistrationFee = t.RegistrationFee,
            IsPublished = t.IsPublished,
            CreatedAt = t.CreatedAt
        }).ToList();

        var response = new TournamentSearchResponse
        {
            Items = items,
            TotalRecords = totalCount,
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

        return Result<TournamentSearchResponse>.Success(response);
    }
}
