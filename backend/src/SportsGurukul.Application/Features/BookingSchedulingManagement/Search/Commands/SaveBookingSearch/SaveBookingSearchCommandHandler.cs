using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Commands.SaveBookingSearch;

public class SaveBookingSearchCommandHandler
    : IRequestHandler<SaveBookingSearchCommand, Result<SavedBookingSearchDto>>
{
    private readonly ISavedSearchRepository _savedSearchRepository;
    private readonly ILogger<SaveBookingSearchCommandHandler> _logger;

    public SaveBookingSearchCommandHandler(
        ISavedSearchRepository savedSearchRepository,
        ILogger<SaveBookingSearchCommandHandler> logger)
    {
        _savedSearchRepository = savedSearchRepository;
        _logger = logger;
    }

    public async Task<Result<SavedBookingSearchDto>> Handle(
        SaveBookingSearchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Saving booking search '{Name}' for user {UserId}",
            request.Name, request.UserId);

        var filters = new BookingSearchFilterDto
        {
            SearchTerm = request.SearchTerm,
            BookingNumber = request.BookingNumber,
            Title = request.Title,
            AcademyId = request.AcademyId,
            BranchId = request.BranchId,
            FacilityId = request.FacilityId,
            CoachId = request.CoachId,
            AthleteId = request.AthleteId,
            BookingType = request.BookingType,
            Status = request.Status,
            ApprovalStatus = request.ApprovalStatus,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            StartTimeFrom = request.StartTimeFrom,
            StartTimeTo = request.StartTimeTo
        };

        var entity = new SavedSearch
        {
            UserId = request.UserId,
            Name = request.Name,
            UsageCount = 0
        };
        entity.SetFilters(filters);

        var saved = await _savedSearchRepository.AddAsync(entity, cancellationToken);

        var dto = new SavedBookingSearchDto
        {
            Id = saved.Id,
            Name = saved.Name,
            Filters = filters,
            UsageCount = saved.UsageCount,
            CreatedAt = saved.CreatedAt
        };

        return Result<SavedBookingSearchDto>.Success(dto);
    }
}
