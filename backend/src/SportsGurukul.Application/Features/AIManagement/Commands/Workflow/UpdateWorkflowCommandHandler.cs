using MediatR;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Workflow;

public class UpdateWorkflowCommandHandler : IRequestHandler<UpdateWorkflowCommand, Result<WorkflowDto>>
{
    private readonly IWorkflowDefinitionRepository _workflowRepo;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWorkflowCommandHandler(IWorkflowDefinitionRepository workflowRepo, IUnitOfWork unitOfWork)
    {
        _workflowRepo = workflowRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WorkflowDto>> Handle(UpdateWorkflowCommand request, CancellationToken cancellationToken)
    {
        var entity = await _workflowRepo.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<WorkflowDto>.Failure("Workflow not found");

        if (request.Name is not null)
            entity.Name = request.Name;
        if (request.Description is not null)
            entity.Description = request.Description;
        if (request.Steps is not null)
            entity.Steps = request.Steps;
        if (request.Triggers is not null)
            entity.Triggers = request.Triggers;
        if (request.Conditions is not null)
            entity.Conditions = request.Conditions;
        if (request.Variables is not null)
            entity.Variables = request.Variables;

        entity.Version++;
        _workflowRepo.Update(entity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<WorkflowDto>.Success(new WorkflowDto(
            entity.Id, entity.Name, entity.Description, entity.Status,
            entity.Steps, entity.Triggers, entity.Conditions, entity.Variables,
            entity.Version, entity.CreatedAt, entity.UpdatedAt
        ));
    }
}
