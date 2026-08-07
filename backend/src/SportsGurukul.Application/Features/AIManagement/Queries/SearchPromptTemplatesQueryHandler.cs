using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class SearchPromptTemplatesQueryHandler : IRequestHandler<SearchPromptTemplatesQuery, Result<IReadOnlyList<PromptTemplateDto>>>
{
    private readonly IPromptService _promptService;

    public SearchPromptTemplatesQueryHandler(IPromptService promptService)
    {
        _promptService = promptService;
    }

    public Task<Result<IReadOnlyList<PromptTemplateDto>>> Handle(SearchPromptTemplatesQuery request, CancellationToken cancellationToken)
        => _promptService.SearchAsync(
            request.SearchTerm,
            request.AssistantId,
            request.PromptType,
            request.IsActive,
            request.Page,
            request.PageSize,
            cancellationToken);
}
