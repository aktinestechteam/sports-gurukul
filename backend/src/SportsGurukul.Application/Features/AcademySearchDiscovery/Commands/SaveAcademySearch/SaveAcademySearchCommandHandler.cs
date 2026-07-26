using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.SaveAcademySearch;

public class SaveAcademySearchCommandHandler : IRequestHandler<SaveAcademySearchCommand, Result<SavedAcademySearchDto>>
{
    private readonly IAcademySearchRepository _academySearchRepository;
    private readonly ILogger<SaveAcademySearchCommandHandler> _logger;

    public SaveAcademySearchCommandHandler(
        IAcademySearchRepository academySearchRepository,
        ILogger<SaveAcademySearchCommandHandler> logger)
    {
        _academySearchRepository = academySearchRepository;
        _logger = logger;
    }

    public async Task<Result<SavedAcademySearchDto>> Handle(SaveAcademySearchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Saving academy search for user: {UserId}, Name: {SearchName}", request.UserId, request.SearchName);

        var savedSearch = new SavedAcademySearch
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            SearchName = request.SearchName,
            SearchTerm = request.SearchTerm,
            Name = request.Name,
            City = request.City,
            State = request.State,
            Country = request.Country,
            District = request.District,
            PinCode = request.PinCode,
            SportName = request.SportName,
            SportCategory = request.SportCategory,
            FacilityType = request.FacilityType,
            HasSwimmingPool = request.HasSwimmingPool,
            HasIndoorStadium = request.HasIndoorStadium,
            HasCricketGround = request.HasCricketGround,
            HasFootballGround = request.HasFootballGround,
            HasGym = request.HasGym,
            HasYogaHall = request.HasYogaHall,
            HasParking = request.HasParking,
            HasMedicalRoom = request.HasMedicalRoom,
            HasWifi = request.HasWifi,
            HasCafeteria = request.HasCafeteria,
            VerifiedOnly = request.VerifiedOnly,
            GovernmentRegisteredOnly = request.GovernmentRegisteredOnly,
            OpenNow = request.OpenNow,
            WeekendOpen = request.WeekendOpen,
            MinMembershipPrice = request.MinMembershipPrice,
            MaxMembershipPrice = request.MaxMembershipPrice,
            MinRating = request.MinRating,
            ResultCount = request.ResultCount
        };

        await _academySearchRepository.SaveSearchAsync(savedSearch, cancellationToken);

        return Result<SavedAcademySearchDto>.Success(new SavedAcademySearchDto
        {
            Id = savedSearch.Id,
            SearchName = savedSearch.SearchName,
            SearchTerm = savedSearch.SearchTerm,
            City = savedSearch.City,
            State = savedSearch.State,
            SportName = savedSearch.SportName,
            FacilityType = savedSearch.FacilityType,
            VerifiedOnly = savedSearch.VerifiedOnly,
            MinMembershipPrice = savedSearch.MinMembershipPrice,
            MaxMembershipPrice = savedSearch.MaxMembershipPrice,
            ResultCount = savedSearch.ResultCount,
            CreatedAt = savedSearch.CreatedAt
        });
    }
}
