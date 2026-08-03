using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class GetPromptTemplateByIdQueryHandler : IRequestHandler<GetPromptTemplateByIdQuery, Result<PromptTemplateDto>>
{
    private readonly IPromptService _promptService;

    public GetPromptTemplateByIdQueryHandler(IPromptService promptService)
    {
        _promptService = promptService;
    }

    public Task<Result<PromptTemplateDto>> Handle(GetPromptTemplateByIdQuery request, CancellationToken cancellationToken)
        => _promptService.GetByIdAsync(request.PromptTemplateId, cancellationToken);
}
