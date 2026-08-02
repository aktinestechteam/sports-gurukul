using MediatR;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Assistant;

public class AssignToolsCommandHandler : IRequestHandler<AssignToolsCommand, Result<AssistantDto>>
{
    private readonly IAssistantService _assistantService;
    private readonly IUnitOfWork _unitOfWork;

    public AssignToolsCommandHandler(IAssistantService assistantService, IUnitOfWork unitOfWork)
    {
        _assistantService = assistantService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AssistantDto>> Handle(AssignToolsCommand request, CancellationToken cancellationToken)
    {
        var result = await _assistantService.AssignToolsAsync(request.AssistantId, request.ToolIds, cancellationToken);
        if (!result.IsSuccess)
            return Result<AssistantDto>.Failure(result.Error!);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AssistantDto>.Success(default!);
    }
}
