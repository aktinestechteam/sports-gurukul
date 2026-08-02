using MediatR;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public class DeleteConversationCommandHandler : IRequestHandler<DeleteConversationCommand, Result<ConversationDto>>
{
    private readonly IConversationService _conversationService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteConversationCommandHandler(IConversationService conversationService, IUnitOfWork unitOfWork)
    {
        _conversationService = conversationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ConversationDto>> Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
    {
        var result = await _conversationService.DeleteAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return Result<ConversationDto>.Failure(result.Error!);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ConversationDto>.Success(default!);
    }
}
