using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Template;

public record CreateTemplateVersionCommand(
    Guid TemplateId,
    string SubjectTemplate,
    string BodyTemplate,
    string? ChangeNotes
) : IRequest<Result<TemplateVersionDto>>;
