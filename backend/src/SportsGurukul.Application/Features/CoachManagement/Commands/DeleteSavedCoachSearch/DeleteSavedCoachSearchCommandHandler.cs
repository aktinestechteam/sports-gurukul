using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeleteSavedCoachSearch;

public class DeleteSavedCoachSearchCommandHandler : IRequestHandler<DeleteSavedCoachSearchCommand, Result<Unit>>
{
    private readonly ISavedSearchRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSavedCoachSearchCommandHandler> _logger;

    public DeleteSavedCoachSearchCommandHandler(
        ISavedSearchRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteSavedCoachSearchCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteSavedCoachSearchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting saved coach search: {Id} for user: {UserId}", request.Id, request.UserId);

        var savedSearch = await _repository.GetByIdAndUserAsync(request.Id, request.UserId, cancellationToken);
        if (savedSearch is null)
            return Result<Unit>.Failure("Saved search not found.");

        _repository.Remove(savedSearch);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
