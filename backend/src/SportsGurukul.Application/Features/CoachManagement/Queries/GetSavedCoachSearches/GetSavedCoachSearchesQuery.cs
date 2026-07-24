using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetSavedCoachSearches;

public class GetSavedCoachSearchesQuery : IRequest<Result<IReadOnlyList<SavedSearchDto>>>
{
    public Guid UserId { get; set; }
}
