using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public record GetWorkflowByIdQuery(Guid WorkflowId) : IRequest<Result<WorkflowDto>>;
