using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;

public record AttachDocumentCommand(
    Guid KnowledgeBaseId,
    string Title,
    AIKnowledgeDocumentType DocumentType,
    string? Content,
    string? ExternalId,
    string? StoragePath,
    string? MimeType,
    string? MetadataJson
) : IRequest<Result<KnowledgeDocumentDto>>;
