using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Template;

public record PublishTemplateCommand(Guid Id) : IRequest<Result<TemplateDto>>;
