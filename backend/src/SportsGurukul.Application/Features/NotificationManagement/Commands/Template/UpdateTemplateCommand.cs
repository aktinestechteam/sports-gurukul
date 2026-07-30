using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Template;

public record UpdateTemplateCommand(
    Guid Id,
    string? Name,
    string? Description,
    string? SubjectTemplate,
    string? BodyTemplate,
    List<CreateTemplateVariableRequest>? Variables
) : IRequest<Result<TemplateDto>>;
