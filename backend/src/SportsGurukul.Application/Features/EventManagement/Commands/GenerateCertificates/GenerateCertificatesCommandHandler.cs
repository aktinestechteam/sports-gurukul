using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.GenerateCertificates;

public class GenerateCertificatesCommandHandler : IRequestHandler<GenerateCertificatesCommand, Result<List<CertificateDto>>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventCertificateService _certificateService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GenerateCertificatesCommandHandler> _logger;

    public GenerateCertificatesCommandHandler(
        IEventRepository eventRepository,
        IEventCertificateService certificateService,
        IUnitOfWork unitOfWork,
        ILogger<GenerateCertificatesCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _certificateService = certificateService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<CertificateDto>>> Handle(GenerateCertificatesCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating certificates for event: {EventId}", request.EventId);

        var evt = await _eventRepository.GetWithDetailsAsync(request.EventId, cancellationToken);
        if (evt is null)
            return Result<List<CertificateDto>>.Failure("Event not found.");

        if (evt.Status != EventStatus.Completed)
            return Result<List<CertificateDto>>.Failure("Certificates can only be generated for completed events.");

        var eligibleParticipants = await _certificateService.GetEligibleParticipantsAsync(request.EventId, cancellationToken);
        if (eligibleParticipants.Count == 0)
            return Result<List<CertificateDto>>.Failure("No eligible participants found for certificate generation.");

        var certificates = new List<EventCertificate>();

        foreach (var participant in eligibleParticipants)
        {
            var certificateNumber = await _certificateService.GenerateCertificateNumberAsync(cancellationToken);

            var certificate = new EventCertificate
            {
                Id = Guid.NewGuid(),
                EventId = request.EventId,
                ParticipantId = participant.Id,
                CertificateNumber = certificateNumber,
                CertificateType = "Participation",
                IssuedDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            evt.Certificates.Add(certificate);
            certificates.Add(certificate);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Generated {Count} certificates for event: {EventId}", certificates.Count, request.EventId);

        var dtos = certificates.Select(c =>
        {
            var participant = eligibleParticipants.FirstOrDefault(p => p.Id == c.ParticipantId);
            return MapToDto(c, evt.EventName, participant?.ParticipantName ?? string.Empty);
        }).ToList();

        return Result<List<CertificateDto>>.Success(dtos);
    }

    internal static CertificateDto MapToDto(EventCertificate cert, string eventName = "", string participantName = "")
    {
        return new CertificateDto
        {
            Id = cert.Id,
            EventId = cert.EventId,
            EventName = eventName,
            ParticipantId = cert.ParticipantId,
            ParticipantName = participantName,
            CertificateNumber = cert.CertificateNumber,
            CertificateType = cert.CertificateType,
            IssuedDate = cert.IssuedDate,
            IssuedBy = cert.IssuedBy,
            DocumentUrl = cert.DocumentUrl,
            IsPrinted = cert.IsPrinted,
            IsSent = cert.IsSent,
            Notes = cert.Notes,
            CreatedAt = cert.CreatedAt
        };
    }
}
