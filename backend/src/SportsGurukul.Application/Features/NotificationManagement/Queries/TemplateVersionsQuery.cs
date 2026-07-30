using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Queries;

public record TemplateVersionsQuery(Guid TemplateId) : IRequest<Result<List<TemplateVersionDto>>>;
