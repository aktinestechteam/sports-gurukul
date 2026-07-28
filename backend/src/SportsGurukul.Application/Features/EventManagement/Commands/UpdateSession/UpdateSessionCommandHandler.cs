using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.CreateSession;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.UpdateSession;

public class UpdateSessionCommandHandler : IRequestHandler<UpdateSessionCommand, Result<EventSessionDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateSessionCommandHandler> _logger;

    public UpdateSessionCommandHandler(
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateSessionCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EventSessionDto>> Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating session {SessionId}", request.SessionId);

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

        session.Title = request.Title;
        session.Description = request.Description;
        session.SessionDate = request.SessionDate;
        session.StartTime = request.StartTime;
        session.EndTime = request.EndTime;
        session.VenueId = request.VenueId;
        session.SpeakerId = request.SpeakerId;
        session.CoachId = request.CoachId;
        session.Capacity = request.Capacity;
        session.IsBreak = request.IsBreak;
        session.Notes = request.Notes;
        session.UpdatedAt = DateTime.UtcNow;

        var evt = await _eventRepository.GetWithDetailsAsync(session.EventId, cancellationToken);
        if (evt is not null)
        {
            _eventRepository.Update(evt);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Session {SessionId} updated", request.SessionId);

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
