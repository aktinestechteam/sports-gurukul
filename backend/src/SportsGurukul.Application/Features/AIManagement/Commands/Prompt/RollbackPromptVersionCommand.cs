using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Prompt;

public record RollbackPromptVersionCommand(Guid PromptTemplateId, int VersionNumber) : IRequest<Result<PromptTemplateDto>>;
