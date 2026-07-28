using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.CreateEvent;

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Result<EventDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventLifecycleService _eventLifecycleService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateEventCommandHandler> _logger;

    public CreateEventCommandHandler(
        IEventRepository eventRepository,
        IEventLifecycleService eventLifecycleService,
        IUnitOfWork unitOfWork,
        ILogger<CreateEventCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _eventLifecycleService = eventLifecycleService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EventDto>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating event: {EventName}", request.EventName);

        if (request.EndDate <= request.StartDate)
        {
            _logger.LogWarning("Event end date must be after start date");
            return Result<EventDto>.Failure("Event end date must be after start date.");
        }

        if (request.RegistrationCloseDate >= request.StartDate)
        {
            _logger.LogWarning("Registration close date must be before event start date");
            return Result<EventDto>.Failure("Registration close date must be before event start date.");
        }

        var eventCode = await _eventLifecycleService.GenerateEventCodeAsync(cancellationToken);

        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            EventCode = eventCode,
            EventName = request.EventName,
            Description = request.Description,
            ShortDescription = request.ShortDescription,
            AcademyId = request.AcademyId,
            SportId = request.SportId,
            EventTypeId = request.EventTypeId,
            EventCategoryId = request.EventCategoryId,
            Status = EventStatus.Draft,
            RegistrationType = request.RegistrationType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            RegistrationOpenDate = request.RegistrationOpenDate,
            RegistrationCloseDate = request.RegistrationCloseDate,
            MaxParticipants = request.MaxParticipants,
            MinParticipants = request.MinParticipants,
            RegistrationFee = request.RegistrationFee,
            IsFeatured = request.IsFeatured,
            IsPublic = request.IsPublic,
            BannerUrl = request.BannerUrl,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            Website = request.Website,
            Tags = request.Tags,
            Requirements = request.Requirements,
            WhatToBring = request.WhatToBring,
            CancellationPolicy = request.CancellationPolicy,
            CreatedAt = DateTime.UtcNow
        };

        await _eventRepository.AddAsync(eventEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Event created: {EventId}, Code: {EventCode}", eventEntity.Id, eventCode);

        var dto = MapToDto(eventEntity);
        return Result<EventDto>.Success(dto);
    }

    internal static EventDto MapToDto(Event eventEntity)
    {
        return EventDto.MapToDto(
            eventEntity,
            eventEntity.Academy?.Name,
            eventEntity.Sport?.Name,
            eventEntity.EventType?.Name,
            eventEntity.EventCategory?.Name);
    }
}
