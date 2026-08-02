using MediatR;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Agent;

public class UpdateAgentCommandHandler : IRequestHandler<UpdateAgentCommand, Result<AgentDto>>
{
    private readonly IAgentService _agentService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAgentCommandHandler(IAgentService agentService, IUnitOfWork unitOfWork)
    {
        _agentService = agentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AgentDto>> Handle(UpdateAgentCommand request, CancellationToken cancellationToken)
    {
        var updateRequest = new UpdateAgentRequest(
            request.Id, request.Name, request.Description, request.Configuration,
            request.Tools, request.Rules, request.Constraints,
            request.MaxIterations, request.RequiresApproval);
        var result = await _agentService.UpdateAsync(updateRequest, cancellationToken);
        if (!result.IsSuccess)
            return Result<AgentDto>.Failure(result.Error!);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var a = result.Value!;
        return Result<AgentDto>.Success(new AgentDto(
            a.Id, a.Name, a.Description, a.AssistantId, a.Assistant?.Name,
            a.Status, a.Configuration, a.Tools, a.Rules, a.Constraints,
            a.MaxIterations, a.RequiresApproval, a.CreatedAt, a.UpdatedAt,
            []
        ));
    }
}
