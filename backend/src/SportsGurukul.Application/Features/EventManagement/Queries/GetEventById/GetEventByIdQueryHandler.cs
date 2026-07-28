using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Queries.GetEventById;

public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, Result<EventDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<GetEventByIdQueryHandler> _logger;

    public GetEventByIdQueryHandler(
        IEventRepository eventRepository,
        ILogger<GetEventByIdQueryHandler> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<Result<EventDto>> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting event by ID: {EventId}", request.EventId);

        var evt = await _eventRepository.GetWithDetailsAsync(request.EventId, cancellationToken);
        if (evt is null)
        {
            _logger.LogWarning("Event not found: {EventId}", request.EventId);
            return Result<EventDto>.Failure("Event not found.");
        }

        var dto = EventDto.MapToDto(evt);
        return Result<EventDto>.Success(dto);
    }
}
