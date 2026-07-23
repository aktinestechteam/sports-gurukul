using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.CreateSavedSearch;

public class CreateSavedSearchCommandHandler : IRequestHandler<CreateSavedSearchCommand, Result<SavedSearchDto>>
{
    private readonly ISavedSearchRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateSavedSearchCommandHandler> _logger;

    public CreateSavedSearchCommandHandler(
        ISavedSearchRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CreateSavedSearchCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<SavedSearchDto>> Handle(CreateSavedSearchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating saved search for user: {UserId}, Name: {Name}", request.UserId, request.Name);

        var savedSearch = new SavedSearch
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Name = request.Name,
            FiltersJson = request.FiltersJson,
            UsageCount = 0
        };

        await _repository.AddAsync(savedSearch, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<SavedSearchDto>.Success(new SavedSearchDto
        {
            Id = savedSearch.Id,
            Name = savedSearch.Name,
            FiltersJson = savedSearch.FiltersJson,
            UsageCount = savedSearch.UsageCount,
            CreatedAt = savedSearch.CreatedAt
        });
    }
}
