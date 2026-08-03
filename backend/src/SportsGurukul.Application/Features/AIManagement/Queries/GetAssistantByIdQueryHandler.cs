using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class GetAssistantByIdQueryHandler : IRequestHandler<GetAssistantByIdQuery, Result<AssistantDto>>
{
    private readonly IAssistantService _assistantService;

    public GetAssistantByIdQueryHandler(IAssistantService assistantService)
    {
        _assistantService = assistantService;
    }

    public Task<Result<AssistantDto>> Handle(GetAssistantByIdQuery request, CancellationToken cancellationToken)
        => _assistantService.GetByIdAsync(request.AssistantId, cancellationToken);
}
