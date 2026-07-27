using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetCertificatesByAthleteQuery;

public class GetCertificatesByAthleteQueryHandler : IRequestHandler<GetCertificatesByAthleteQuery, Result<IReadOnlyList<CertificateDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetCertificatesByAthleteQueryHandler> _logger;

    public GetCertificatesByAthleteQueryHandler(
        IApplicationDbContext context,
        ILogger<GetCertificatesByAthleteQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<CertificateDto>>> Handle(GetCertificatesByAthleteQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting certificates for athlete ID: {AthleteId}", request.AthleteId);

        var certificates = await _context.Certificates
            .Include(c => c.Enrollment)
            .Where(c => c.Enrollment.AthleteId == request.AthleteId)
            .ToListAsync(cancellationToken);

        var dtos = certificates.Select(CertificateDto.MapToDto).ToList();

        return Result<IReadOnlyList<CertificateDto>>.Success(dtos);
    }
}
