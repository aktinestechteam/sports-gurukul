using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetAcademyStatistics;

public class GetAcademyStatisticsQuery : IRequest<Result<AcademyStatisticsDto>>
{
    public Guid AcademyId { get; set; }
}
