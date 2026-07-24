using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.SaveCoachSearch;

public class SaveCoachSearchCommand : IRequest<Result<SavedSearchDto>>
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FiltersJson { get; set; } = "{}";
}
