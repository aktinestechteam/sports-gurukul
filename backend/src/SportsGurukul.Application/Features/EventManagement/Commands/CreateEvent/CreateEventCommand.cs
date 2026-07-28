using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.CreateEvent;

public class CreateEventCommand : IRequest<Result<EventDto>>
{
    public string EventName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public Guid AcademyId { get; set; }
    public Guid SportId { get; set; }
    public Guid EventTypeId { get; set; }
    public Guid? EventCategoryId { get; set; }
    public EventRegistrationType RegistrationType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime RegistrationOpenDate { get; set; }
    public DateTime RegistrationCloseDate { get; set; }
    public int? MaxParticipants { get; set; }
    public int? MinParticipants { get; set; }
    public decimal? RegistrationFee { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPublic { get; set; } = true;
    public string? BannerUrl { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Website { get; set; }
    public string? Tags { get; set; }
    public string? Requirements { get; set; }
    public string? WhatToBring { get; set; }
    public string? CancellationPolicy { get; set; }
}
