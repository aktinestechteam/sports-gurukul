using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.CreateSavedSearch;

public class CreateSavedSearchCommand : IRequest<Result<SavedSearchDto>>
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FiltersJson { get; set; } = "{}";
}
