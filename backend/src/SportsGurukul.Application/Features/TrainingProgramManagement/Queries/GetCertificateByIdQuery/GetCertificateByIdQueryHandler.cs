using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetCertificateByIdQuery;

public class GetCertificateByIdQueryHandler : IRequestHandler<GetCertificateByIdQuery, Result<CertificateDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetCertificateByIdQueryHandler> _logger;

    public GetCertificateByIdQueryHandler(
        IApplicationDbContext context,
        ILogger<GetCertificateByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<CertificateDto>> Handle(GetCertificateByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting certificate by ID: {Id}", request.Id);

        var certificate = await _context.Certificates
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (certificate == null)
        {
            return Result<CertificateDto>.Failure($"Certificate with ID {request.Id} not found.");
        }

        var dto = CertificateDto.MapToDto(certificate);

        return Result<CertificateDto>.Success(dto);
    }
}
