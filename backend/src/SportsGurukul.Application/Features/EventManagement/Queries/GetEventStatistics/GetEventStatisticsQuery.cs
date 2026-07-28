using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Queries.GetEventStatistics;

public class GetEventStatisticsQuery : IRequest<Result<StatisticsDto>>
{
    public Guid EventId { get; set; }
}
