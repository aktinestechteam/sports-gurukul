using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.IssueCertificate;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.EventManagement.Commands.RevokeCertificate;

public class RevokeCertificateCommandHandler : IRequestHandler<RevokeCertificateCommand, Result<CertificateDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RevokeCertificateCommandHandler> _logger;

    public RevokeCertificateCommandHandler(
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork,
        ILogger<RevokeCertificateCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CertificateDto>> Handle(RevokeCertificateCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Revoking certificate: {CertificateId}", request.CertificateId);

        var events = await _eventRepository.GetAllAsync(cancellationToken);
        EventCertificate? certificate = null;
        string eventName = string.Empty;
        string participantName = string.Empty;

        foreach (var evt in events)
        {
            var evtWithDetails = await _eventRepository.GetWithDetailsAsync(evt.Id, cancellationToken);
            if (evtWithDetails is null) continue;

            certificate = evtWithDetails.Certificates.FirstOrDefault(c => c.Id == request.CertificateId);
            if (certificate is not null)
            {
                eventName = evtWithDetails.EventName;
                participantName = certificate.Participant?.ParticipantName ?? string.Empty;
                break;
            }
        }

        if (certificate is null)
            return Result<CertificateDto>.Failure("Certificate not found.");

        certificate.Notes = request.Reason;
        certificate.IsDeleted = true;
        certificate.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Certificate revoked: {CertificateId}", request.CertificateId);

        var dto = IssueCertificateCommandHandler.MapToDto(certificate, eventName, participantName);
        return Result<CertificateDto>.Success(dto);
    }
}
