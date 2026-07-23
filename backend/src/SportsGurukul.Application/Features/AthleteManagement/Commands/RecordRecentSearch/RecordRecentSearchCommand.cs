using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.RecordRecentSearch;

public class RecordRecentSearchCommand : IRequest<Result<Unit>>
{
    public Guid UserId { get; set; }
    public string QueryText { get; set; } = string.Empty;
    public string FiltersJson { get; set; } = "{}";
    public int ResultCount { get; set; }
}
