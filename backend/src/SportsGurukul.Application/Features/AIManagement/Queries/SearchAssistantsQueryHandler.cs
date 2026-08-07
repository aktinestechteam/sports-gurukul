using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class SearchAssistantsQueryHandler : IRequestHandler<SearchAssistantsQuery, Result<IReadOnlyList<AssistantDto>>>
{
    private readonly IAssistantService _assistantService;

    public SearchAssistantsQueryHandler(IAssistantService assistantService)
    {
        _assistantService = assistantService;
    }

    public Task<Result<IReadOnlyList<AssistantDto>>> Handle(SearchAssistantsQuery request, CancellationToken cancellationToken)
        => _assistantService.SearchAsync(
            request.SearchTerm,
            request.AssistantType,
            request.OwnerUserId,
            request.IsActive,
            request.Page,
            request.PageSize,
            cancellationToken);
}
