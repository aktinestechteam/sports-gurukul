using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.CreateSession;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.AssignSpeaker;

public class AssignSpeakerCommandHandler : IRequestHandler<AssignSpeakerCommand, Result<EventSessionDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignSpeakerCommandHandler> _logger;

    public AssignSpeakerCommandHandler(
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignSpeakerCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EventSessionDto>> Handle(AssignSpeakerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning speaker {SpeakerId} to session {SessionId}", request.SpeakerId, request.SessionId);

        var session = await FindSessionAsync(request.SessionId, cancellationToken);
        if (session is null)
        {
            _logger.LogWarning("Session {SessionId} not found", request.SessionId);
            return Result<EventSessionDto>.Failure("Session not found.");
        }

        session.SpeakerId = request.SpeakerId;
        session.UpdatedAt = DateTime.UtcNow;

        var evt = await _eventRepository.GetWithDetailsAsync(session.EventId, cancellationToken);
        if (evt is not null)
        {
            _eventRepository.Update(evt);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Speaker {SpeakerId} assigned to session {SessionId}", request.SpeakerId, request.SessionId);

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
