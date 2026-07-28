using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.CreateEvent;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.UpdateEvent;

public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, Result<EventDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateEventCommandHandler> _logger;

    public UpdateEventCommandHandler(
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateEventCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EventDto>> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating event: {EventId}", request.EventId);

        var eventEntity = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
        if (eventEntity is null)
        {
            _logger.LogWarning("Event not found: {EventId}", request.EventId);
            return Result<EventDto>.Failure("Event not found.");
        }

        if (request.EventName is not null) eventEntity.EventName = request.EventName;
        if (request.Description is not null) eventEntity.Description = request.Description;
        if (request.ShortDescription is not null) eventEntity.ShortDescription = request.ShortDescription;
        if (request.AcademyId.HasValue) eventEntity.AcademyId = request.AcademyId.Value;
        if (request.SportId.HasValue) eventEntity.SportId = request.SportId.Value;
        if (request.EventTypeId.HasValue) eventEntity.EventTypeId = request.EventTypeId.Value;
        if (request.EventCategoryId.HasValue) eventEntity.EventCategoryId = request.EventCategoryId;
        if (request.RegistrationType.HasValue) eventEntity.RegistrationType = request.RegistrationType.Value;
        if (request.StartDate.HasValue) eventEntity.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue) eventEntity.EndDate = request.EndDate.Value;
        if (request.RegistrationOpenDate.HasValue) eventEntity.RegistrationOpenDate = request.RegistrationOpenDate.Value;
        if (request.RegistrationCloseDate.HasValue) eventEntity.RegistrationCloseDate = request.RegistrationCloseDate.Value;
        if (request.MaxParticipants.HasValue) eventEntity.MaxParticipants = request.MaxParticipants;
        if (request.MinParticipants.HasValue) eventEntity.MinParticipants = request.MinParticipants;
        if (request.RegistrationFee.HasValue) eventEntity.RegistrationFee = request.RegistrationFee;
        if (request.IsFeatured.HasValue) eventEntity.IsFeatured = request.IsFeatured.Value;
        if (request.IsPublic.HasValue) eventEntity.IsPublic = request.IsPublic.Value;
        if (request.BannerUrl is not null) eventEntity.BannerUrl = request.BannerUrl;
        if (request.ContactEmail is not null) eventEntity.ContactEmail = request.ContactEmail;
        if (request.ContactPhone is not null) eventEntity.ContactPhone = request.ContactPhone;
        if (request.Website is not null) eventEntity.Website = request.Website;
        if (request.Tags is not null) eventEntity.Tags = request.Tags;
        if (request.Requirements is not null) eventEntity.Requirements = request.Requirements;
        if (request.WhatToBring is not null) eventEntity.WhatToBring = request.WhatToBring;
        if (request.CancellationPolicy is not null) eventEntity.CancellationPolicy = request.CancellationPolicy;

        eventEntity.UpdatedAt = DateTime.UtcNow;

        _eventRepository.Update(eventEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Event updated: {EventId}", request.EventId);

        var dto = CreateEventCommandHandler.MapToDto(eventEntity);
        return Result<EventDto>.Success(dto);
    }
}
