using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Queries.GetCertificatesByEvent;

public class GetCertificatesByEventQueryHandler : IRequestHandler<GetCertificatesByEventQuery, Result<List<CertificateDto>>>
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<GetCertificatesByEventQueryHandler> _logger;

    public GetCertificatesByEventQueryHandler(
        IEventRepository eventRepository,
        ILogger<GetCertificatesByEventQueryHandler> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<Result<List<CertificateDto>>> Handle(GetCertificatesByEventQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting certificates for event: {EventId}", request.EventId);

        var evt = await _eventRepository.GetWithDetailsAsync(request.EventId, cancellationToken);
        if (evt is null)
        {
            _logger.LogWarning("Event not found: {EventId}", request.EventId);
            return Result<List<CertificateDto>>.Failure("Event not found.");
        }

        var certificates = (evt.Certificates?.ToList() ?? []).Select(c => new CertificateDto
        {
            Id = c.Id,
            EventId = c.EventId,
            EventName = evt.EventName,
            ParticipantId = c.ParticipantId,
            ParticipantName = c.Participant?.ParticipantName ?? string.Empty,
            CertificateNumber = c.CertificateNumber,
            CertificateType = c.CertificateType,
            IssuedDate = c.IssuedDate,
            IssuedBy = c.IssuedBy,
            DocumentUrl = c.DocumentUrl,
            IsPrinted = c.IsPrinted,
            IsSent = c.IsSent,
            Notes = c.Notes,
            CreatedAt = c.CreatedAt
        }).ToList();

        return Result<List<CertificateDto>>.Success(certificates);
    }
}
