using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Queries.GetWaitlistPosition;

public class GetWaitlistPositionQueryHandler : IRequestHandler<GetWaitlistPositionQuery, Result<PlatformWaitlistDto>>
{
    private readonly IEventRegistrationRepository _registrationRepository;
    private readonly ILogger<GetWaitlistPositionQueryHandler> _logger;

    public GetWaitlistPositionQueryHandler(
        IEventRegistrationRepository registrationRepository,
        ILogger<GetWaitlistPositionQueryHandler> logger)
    {
        _registrationRepository = registrationRepository;
        _logger = logger;
    }

    public async Task<Result<PlatformWaitlistDto>> Handle(GetWaitlistPositionQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching waitlist position for participant {ParticipantId} on program {ProgramId}",
            request.ParticipantId, request.ProgramId);

        var waitlisted = await _registrationRepository.GetByEventIdWithStatusAsync(
            request.ProgramId, EventRegistrationStatus.Waitlisted, cancellationToken);

        var registration = waitlisted.FirstOrDefault(r =>
            (r.AthleteId == request.ParticipantId) || (r.UserId == request.ParticipantId));

        if (registration is null)
            return Result<PlatformWaitlistDto>.Failure("Participant is not on the waitlist for this program.");

        var dto = new PlatformWaitlistDto
        {
            Id = registration.Id,
            ProgramId = registration.EventId,
            AthleteId = registration.AthleteId,
            UserId = registration.UserId,
            Position = registration.WaitlistPosition ?? 0,
            Status = WaitlistStatus.Active,
            RequestedAt = registration.RegistrationDate ?? registration.CreatedAt,
            Notes = registration.Notes,
            CreatedAt = registration.CreatedAt
        };

        _logger.LogInformation("Waitlist position for participant {ParticipantId}: {Position}", request.ParticipantId, dto.Position);
        return Result<PlatformWaitlistDto>.Success(dto);
    }
}
