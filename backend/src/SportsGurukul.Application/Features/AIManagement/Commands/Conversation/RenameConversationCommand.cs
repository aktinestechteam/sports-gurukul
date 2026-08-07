using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public record RenameConversationCommand(Guid ConversationId, string Title) : IRequest<Result<ConversationDto>>;
