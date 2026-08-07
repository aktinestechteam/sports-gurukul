using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;

public record RebuildKnowledgeIndexCommand(Guid KnowledgeBaseId) : IRequest<Result<KnowledgeBaseDto>>;
