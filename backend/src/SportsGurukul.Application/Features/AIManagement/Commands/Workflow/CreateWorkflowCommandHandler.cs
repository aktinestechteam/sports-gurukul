using MediatR;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Workflow;

public class CreateWorkflowCommandHandler : IRequestHandler<CreateWorkflowCommand, Result<WorkflowDto>>
{
    private readonly IWorkflowDefinitionRepository _workflowRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWorkflowCommandHandler(IWorkflowDefinitionRepository workflowRepo, IUnitOfWork unitOfWork)
    {
        _workflowRepo = workflowRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WorkflowDto>> Handle(CreateWorkflowCommand request, CancellationToken cancellationToken)
    {
        var entity = new WorkflowDefinition
        {
            Name = request.Name,
            Description = request.Description,
            Steps = request.Steps,
            Triggers = request.Triggers,
            Conditions = request.Conditions,
            Variables = request.Variables,
            Status = WorkflowStatus.Draft,
            Version = 1
        };

        var created = await _workflowRepo.AddAsync(entity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<WorkflowDto>.Success(new WorkflowDto(
            created.Id, created.Name, created.Description, created.Status,
            created.Steps, created.Triggers, created.Conditions, created.Variables,
            created.Version, created.CreatedAt, created.UpdatedAt
        ));
    }
}
