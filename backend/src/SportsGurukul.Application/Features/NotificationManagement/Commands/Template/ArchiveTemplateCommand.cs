using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Template;

public record ArchiveTemplateCommand(Guid Id) : IRequest<Result<bool>>;
