using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public record ConversationHistoryQuery(Guid ConversationId, int Page = 1, int PageSize = 50) : IRequest<Result<PaginatedResult<MessageDto>>>;
