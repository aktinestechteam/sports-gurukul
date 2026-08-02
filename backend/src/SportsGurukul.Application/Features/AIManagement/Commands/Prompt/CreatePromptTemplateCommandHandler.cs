using MediatR;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

public class CreatePromptTemplateCommandHandler : IRequestHandler<CreatePromptTemplateCommand, Result<PromptTemplateDto>>
{
    private readonly IPromptService _promptService;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePromptTemplateCommandHandler(IPromptService promptService, IUnitOfWork unitOfWork)
    {
        _promptService = promptService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PromptTemplateDto>> Handle(CreatePromptTemplateCommand request, CancellationToken cancellationToken)
    {
        var createRequest = new CreatePromptTemplateRequest(
            request.Name, request.Description, request.Type,
            request.TemplateContent, request.Variables, request.Tags, request.Category);
        var result = await _promptService.CreateAsync(createRequest, cancellationToken);
        if (!result.IsSuccess)
            return Result<PromptTemplateDto>.Failure(result.Error!);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var p = result.Value!;
        return Result<PromptTemplateDto>.Success(new PromptTemplateDto(
            p.Id, p.Name, p.Description, p.Type, p.Status, p.TemplateContent,
            p.Variables, p.Tags, p.Category, p.CurrentVersion, p.CreatedAt, p.UpdatedAt,
            p.Versions?.Select(v => new PromptVersionDto(
                v.Id, v.PromptTemplateId, v.VersionNumber, v.Content, v.ChangeNotes, v.Hash, v.CreatedAt
            )).ToList() ?? []
        ));
    }
}
