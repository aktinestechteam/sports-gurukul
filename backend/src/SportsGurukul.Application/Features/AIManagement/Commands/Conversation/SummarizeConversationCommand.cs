using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public record SummarizeConversationCommand(Guid ConversationId, string Summary) : IRequest<Result<ConversationDto>>;
