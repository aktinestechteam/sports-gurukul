using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Commands.DeleteSavedSearch;

public class DeleteSavedSearchCommand : IRequest<Result<bool>>
{
    public Guid SavedSearchId { get; set; }
    public Guid UserId { get; set; }
}
