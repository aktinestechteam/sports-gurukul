using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.CreateSession;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.AssignCoach;

public class AssignCoachCommandHandler : IRequestHandler<AssignCoachCommand, Result<EventSessionDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignCoachCommandHandler> _logger;

    public AssignCoachCommandHandler(
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignCoachCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EventSessionDto>> Handle(AssignCoachCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning coach {CoachId} to session {SessionId}", request.CoachId, request.SessionId);

        var session = await FindSessionAsync(request.SessionId, cancellationToken);
        if (session is null)
        {
            _logger.LogWarning("Session {SessionId} not found", request.SessionId);
            return Result<EventSessionDto>.Failure("Session not found.");
        }

        session.CoachId = request.CoachId;
        session.UpdatedAt = DateTime.UtcNow;

        var evt = await _eventRepository.GetWithDetailsAsync(session.EventId, cancellationToken);
        if (evt is not null)
        {
            _eventRepository.Update(evt);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Coach {CoachId} assigned to session {SessionId}", request.CoachId, request.SessionId);

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
