using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class PromptQueryHandler
    : IRequestHandler<PromptQuery, Result<PromptTemplateDto>>
{
    private readonly IPromptService _promptService;

    public PromptQueryHandler(IPromptService promptService)
    {
        _promptService = promptService;
    }

    public async Task<Result<PromptTemplateDto>> Handle(PromptQuery request, CancellationToken cancellationToken)
    {
        var result = await _promptService.GetByIdAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return Result<PromptTemplateDto>.Failure(result.Error!);

        var p = result.Value!;
        return Result<PromptTemplateDto>.Success(new PromptTemplateDto(
            p.Id, p.Name, p.Description, p.Type, p.Status,
            p.TemplateContent, p.Variables, p.Tags, p.Category,
            p.CurrentVersion, p.CreatedAt, p.UpdatedAt,
            p.Versions?.Select(v => new PromptVersionDto(
                v.Id, v.PromptTemplateId, v.VersionNumber, v.Content,
                v.ChangeNotes, v.Hash, v.CreatedAt
            )).ToList() ?? []
        ));
    }
}
