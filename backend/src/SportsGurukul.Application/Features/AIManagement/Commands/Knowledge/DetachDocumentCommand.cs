using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;

public record DetachDocumentCommand(Guid KnowledgeBaseId, Guid DocumentId) : IRequest<Result<bool>>;
