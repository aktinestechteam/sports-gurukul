using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.EventManagement.Commands.IssueCertificate;

public class IssueCertificateCommandHandler : IRequestHandler<IssueCertificateCommand, Result<CertificateDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventCertificateService _certificateService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<IssueCertificateCommandHandler> _logger;

    public IssueCertificateCommandHandler(
        IEventRepository eventRepository,
        IEventCertificateService certificateService,
        IUnitOfWork unitOfWork,
        ILogger<IssueCertificateCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _certificateService = certificateService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CertificateDto>> Handle(IssueCertificateCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Issuing certificate for participant: {ParticipantId} in event: {EventId}", request.ParticipantId, request.EventId);

        var evt = await _eventRepository.GetWithDetailsAsync(request.EventId, cancellationToken);
        if (evt is null)
            return Result<CertificateDto>.Failure("Event not found.");

        var participant = evt.Participants.FirstOrDefault(p => p.Id == request.ParticipantId);
        if (participant is null)
            return Result<CertificateDto>.Failure("Participant not found in this event.");

        var certificateNumber = await _certificateService.GenerateCertificateNumberAsync(cancellationToken);

        var certificate = new EventCertificate
        {
            Id = Guid.NewGuid(),
            EventId = request.EventId,
            ParticipantId = request.ParticipantId,
            CertificateNumber = certificateNumber,
            CertificateType = request.CertificateType ?? "Participation",
            IssuedDate = DateTime.UtcNow,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        evt.Certificates.Add(certificate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Certificate issued: {CertificateId}, Number: {CertificateNumber}", certificate.Id, certificateNumber);

        var dto = MapToDto(certificate, evt.EventName, participant.ParticipantName);
        return Result<CertificateDto>.Success(dto);
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
