using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class AssistantQueryHandler
    : IRequestHandler<AssistantQuery, Result<AssistantDto>>
{
    private readonly IAssistantService _assistantService;

    public AssistantQueryHandler(IAssistantService assistantService)
    {
        _assistantService = assistantService;
    }

    public async Task<Result<AssistantDto>> Handle(AssistantQuery request, CancellationToken cancellationToken)
    {
        var result = await _assistantService.GetByIdAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return Result<AssistantDto>.Failure(result.Error!);

        var a = result.Value!;
        return Result<AssistantDto>.Success(new AssistantDto(
            a.Id, a.Name, a.Description, a.AssistantType, a.Personality,
            a.SystemPrompt, a.GreetingMessage, a.AvatarUrl,
            a.IsActive, a.IsPublic, a.MaxHistoryLength,
            a.CreatedAt, a.UpdatedAt, null, null
        ));
    }
}
