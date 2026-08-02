using MediatR;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Assistant;

public class ArchiveAssistantCommandHandler : IRequestHandler<ArchiveAssistantCommand, Result<AssistantDto>>
{
    private readonly IAssistantService _assistantService;
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveAssistantCommandHandler(IAssistantService assistantService, IUnitOfWork unitOfWork)
    {
        _assistantService = assistantService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AssistantDto>> Handle(ArchiveAssistantCommand request, CancellationToken cancellationToken)
    {
        var result = await _assistantService.ArchiveAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return Result<AssistantDto>.Failure(result.Error!);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var a = result.Value!;
        return Result<AssistantDto>.Success(new AssistantDto(
            a.Id, a.Name, a.Description, a.AssistantType, a.Personality,
            a.SystemPrompt, a.GreetingMessage, a.AvatarUrl, a.IsActive, a.IsPublic,
            a.MaxHistoryLength, a.CreatedAt, a.UpdatedAt,
            null, null
        ));
    }
}
