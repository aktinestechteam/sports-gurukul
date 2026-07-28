using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Commands.SaveSearch;

public class SaveSearchCommandHandler : IRequestHandler<SaveSearchCommand, Result<SavedEventSearchDto>>
{
    private readonly IEventSearchRepository _searchRepository;
    private readonly ILogger<SaveSearchCommandHandler> _logger;

    public SaveSearchCommandHandler(
        IEventSearchRepository searchRepository,
        ILogger<SaveSearchCommandHandler> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task<Result<SavedEventSearchDto>> Handle(SaveSearchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Saving search '{SearchName}' for user {UserId}", request.SearchName, request.UserId);

        var savedSearch = new EventSavedSearch
        {
            UserId = request.UserId,
            SearchName = request.SearchName,
            SearchTerm = request.SearchTerm,
            SportName = request.SportName,
            AcademyName = request.AcademyName,
            CoachName = request.CoachName,
            SpeakerName = request.SpeakerName,
            VenueName = request.VenueName,
            City = request.City,
            State = request.State,
            Country = request.Country,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            MinPrice = request.MinPrice,
            MaxPrice = request.MaxPrice,
            EventType = request.EventType,
            Category = request.Category,
            SkillLevel = request.SkillLevel,
            AgeGroup = request.AgeGroup,
            Language = request.Language,
            SortBy = request.SortBy,
            CreatedAt = DateTime.UtcNow
        };

        await _searchRepository.SaveSearchAsync(savedSearch, cancellationToken);

        _logger.LogInformation("Saved search {SearchId} for user {UserId}", savedSearch.Id, request.UserId);

        return Result<SavedEventSearchDto>.Success(new SavedEventSearchDto
        {
            Id = savedSearch.Id,
            SearchName = savedSearch.SearchName,
            SearchTerm = savedSearch.SearchTerm,
            SportName = savedSearch.SportName,
            AcademyName = savedSearch.AcademyName,
            CoachName = savedSearch.CoachName,
            SpeakerName = savedSearch.SpeakerName,
            City = savedSearch.City,
            State = savedSearch.State,
            EventType = savedSearch.EventType,
            Category = savedSearch.Category,
            DateFrom = savedSearch.DateFrom,
            DateTo = savedSearch.DateTo,
            MinPrice = savedSearch.MinPrice,
            MaxPrice = savedSearch.MaxPrice,
            SkillLevel = savedSearch.SkillLevel,
            AgeGroup = savedSearch.AgeGroup,
            Language = savedSearch.Language,
            SortBy = savedSearch.SortBy,
            ResultCount = savedSearch.ResultCount,
            UsageCount = savedSearch.UsageCount,
            CreatedAt = savedSearch.CreatedAt
        });
    }
}
