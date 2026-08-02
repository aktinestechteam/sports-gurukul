using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;

public record UpdateKnowledgeBaseCommand(
    Guid Id,
    string? Name,
    string? Description,
    KnowledgeBaseVisibility? Visibility,
    string? Category,
    string? Tags
) : IRequest<Result<KnowledgeBaseDto>>;
