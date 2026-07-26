using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.DeleteSavedAcademySearch;

public class DeleteSavedAcademySearchCommandHandler : IRequestHandler<DeleteSavedAcademySearchCommand, Result<Unit>>
{
    private readonly IAcademySearchRepository _academySearchRepository;
    private readonly ILogger<DeleteSavedAcademySearchCommandHandler> _logger;

    public DeleteSavedAcademySearchCommandHandler(
        IAcademySearchRepository academySearchRepository,
        ILogger<DeleteSavedAcademySearchCommandHandler> logger)
    {
        _academySearchRepository = academySearchRepository;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteSavedAcademySearchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting saved academy search: {SearchId} for user: {UserId}", request.SearchId, request.UserId);

        await _academySearchRepository.DeleteSavedSearchAsync(request.SearchId, request.UserId, cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
