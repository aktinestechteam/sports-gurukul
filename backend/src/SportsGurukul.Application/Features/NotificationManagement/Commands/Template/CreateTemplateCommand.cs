using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Template;

public record CreateTemplateCommand(
    string Name,
    string? Description,
    NotificationChannelType ChannelType,
    string SubjectTemplate,
    string BodyTemplate,
    List<CreateTemplateVariableRequest>? Variables
) : IRequest<Result<TemplateDto>>;
