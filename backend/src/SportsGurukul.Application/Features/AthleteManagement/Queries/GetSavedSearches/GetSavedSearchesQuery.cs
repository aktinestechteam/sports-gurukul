using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetSavedSearches;

public class GetSavedSearchesQuery : IRequest<Result<IReadOnlyList<SavedSearchDto>>>
{
    public Guid UserId { get; set; }
}
