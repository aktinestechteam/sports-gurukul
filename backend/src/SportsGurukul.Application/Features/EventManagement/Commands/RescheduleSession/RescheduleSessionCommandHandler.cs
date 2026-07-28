using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.CreateSession;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.RescheduleSession;

public class RescheduleSessionCommandHandler : IRequestHandler<RescheduleSessionCommand, Result<EventSessionDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RescheduleSessionCommandHandler> _logger;

    public RescheduleSessionCommandHandler(
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork,
        ILogger<RescheduleSessionCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EventSessionDto>> Handle(RescheduleSessionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rescheduling session {SessionId} to {SessionDate}", request.SessionId, request.SessionDate);

        var session = await FindSessionAsync(request.SessionId, cancellationToken);
        if (session is null)
        {
            _logger.LogWarning("Session {SessionId} not found", request.SessionId);
            return Result<EventSessionDto>.Failure("Session not found.");
        }

        if (request.EndTime <= request.StartTime)
        {
            _logger.LogWarning("Session end time must be after start time");
            return Result<EventSessionDto>.Failure("Session end time must be after start time.");
        }

        session.SessionDate = request.SessionDate;
        session.StartTime = request.StartTime;
        session.EndTime = request.EndTime;
        session.UpdatedAt = DateTime.UtcNow;

        var evt = await _eventRepository.GetWithDetailsAsync(session.EventId, cancellationToken);
        if (evt is not null)
        {
            _eventRepository.Update(evt);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Session {SessionId} rescheduled to {SessionDate}", request.SessionId, request.SessionDate);

        var dto = CreateSessionCommandHandler.MapToDto(session);
        return Result<EventSessionDto>.Success(dto);
    }

    private async Task<Domain.Entities.EventSession?> FindSessionAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var events = await _eventRepository.GetAllAsync(cancellationToken);
        foreach (var evt in events)
        {
            var evtWithDetails = await _eventRepository.GetWithDetailsAsync(evt.Id, cancellationToken);
            if (evtWithDetails is not null)
            {
                var found = evtWithDetails.Sessions.FirstOrDefault(s => s.Id == sessionId);
                if (found is not null) return found;
            }
        }
        return null;
    }
}
