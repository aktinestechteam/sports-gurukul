using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public record DeleteConversationCommand(Guid ConversationId) : IRequest<Result<bool>>;
