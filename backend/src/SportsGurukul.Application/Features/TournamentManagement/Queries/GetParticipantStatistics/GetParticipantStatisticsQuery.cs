using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Application.Features.TournamentManagement.Queries.GetParticipantStatistics;

public class GetParticipantStatisticsQuery : IRequest<Result<ParticipantStatisticsDto>>
{
    public Guid TournamentId { get; set; }
    public Guid ParticipantId { get; set; }
}
