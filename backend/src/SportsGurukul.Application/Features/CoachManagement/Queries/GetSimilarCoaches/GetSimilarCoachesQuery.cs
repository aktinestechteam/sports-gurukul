using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetSimilarCoaches;

public class GetSimilarCoachesQuery : IRequest<Result<IReadOnlyList<SimilarCoachDto>>>
{
    public Guid CoachId { get; set; }
    public int Limit { get; set; } = 5;
}
