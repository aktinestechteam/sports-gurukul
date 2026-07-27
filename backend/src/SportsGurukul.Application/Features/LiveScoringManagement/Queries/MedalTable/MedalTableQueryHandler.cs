using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.LiveScoringManagement.DTOs;
using SportsGurukul.Platform.Competition.Interfaces;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Queries.MedalTable;

public class MedalTableQueryHandler : IRequestHandler<MedalTableQuery, Result<MedalTableDto>>
{
    private readonly IMedalService _medalService;
    private readonly ILogger<MedalTableQueryHandler> _logger;

    public MedalTableQueryHandler(IMedalService medalService, ILogger<MedalTableQueryHandler> logger)
    {
        _medalService = medalService;
        _logger = logger;
    }

    public async Task<Result<MedalTableDto>> Handle(MedalTableQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting medal table for tournament {TournamentId}", request.TournamentId);

        var medalTable = await _medalService.GenerateMedalTableAsync(request.TournamentId, cancellationToken);

        var dto = new MedalTableDto
        {
            TournamentId = medalTable.TournamentId,
            GeneratedAt = medalTable.GeneratedAt,
            Entries = medalTable.Entries.Select(e => new MedalTableEntryDto
            {
                ParticipantId = e.ParticipantId,
                ParticipantName = e.ParticipantName,
                AcademyName = e.AcademyName,
                GoldCount = e.GoldCount,
                SilverCount = e.SilverCount,
                BronzeCount = e.BronzeCount,
                TotalMedals = e.TotalMedals,
                TotalPoints = e.TotalPoints
            }).ToList()
        };

        return Result<MedalTableDto>.Success(dto);
    }
}
