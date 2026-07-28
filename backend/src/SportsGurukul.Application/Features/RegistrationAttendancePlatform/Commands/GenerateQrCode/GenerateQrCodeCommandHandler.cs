using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.GenerateQrCode;

public class GenerateQrCodeCommandHandler : IRequestHandler<GenerateQrCodeCommand, Result<PlatformQrCodeDto>>
{
    private readonly IQrCodeService _qrCodeService;
    private readonly ILogger<GenerateQrCodeCommandHandler> _logger;

    public GenerateQrCodeCommandHandler(
        IQrCodeService qrCodeService,
        ILogger<GenerateQrCodeCommandHandler> logger)
    {
        _qrCodeService = qrCodeService;
        _logger = logger;
    }

    public async Task<Result<PlatformQrCodeDto>> Handle(GenerateQrCodeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating QR code for {Type} on {ProgramType} {ProgramId}, participant {ParticipantId}",
            request.Type, request.ProgramType, request.ProgramId, request.ParticipantId);

        var qrData = await _qrCodeService.GenerateQrCodeDataAsync(
            request.Type, request.ProgramType, request.ProgramId, request.ParticipantId, cancellationToken);

        var expiration = await _qrCodeService.GetExpirationAsync(request.Type, request.ProgramType);

        var encodedPayload = _qrCodeService.EncodePayload(
            request.Type, request.ProgramType, request.ProgramId, request.ParticipantId);

        var dto = new PlatformQrCodeDto
        {
            Id = Guid.NewGuid(),
            Type = request.Type,
            ProgramType = request.ProgramType,
            ProgramId = request.ProgramId,
            ParticipantId = request.ParticipantId,
            QrCodeData = qrData,
            EncodedPayload = encodedPayload,
            GeneratedAt = DateTime.UtcNow,
            ExpiresAt = expiration,
            IsValid = true
        };

        _logger.LogInformation("QR code generated: {QrData}", qrData);
        return Result<PlatformQrCodeDto>.Success(dto);
    }
}
