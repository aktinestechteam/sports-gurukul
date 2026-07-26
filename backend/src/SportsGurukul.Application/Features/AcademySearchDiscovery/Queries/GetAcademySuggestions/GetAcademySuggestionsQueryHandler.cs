using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetAcademySuggestions;

public class GetAcademySuggestionsQueryHandler : IRequestHandler<GetAcademySuggestionsQuery, Result<IReadOnlyList<AcademySuggestionDto>>>
{
    private readonly IAcademySearchRepository _academySearchRepository;
    private readonly ILogger<GetAcademySuggestionsQueryHandler> _logger;

    public GetAcademySuggestionsQueryHandler(
        IAcademySearchRepository academySearchRepository,
        ILogger<GetAcademySuggestionsQueryHandler> logger)
    {
        _academySearchRepository = academySearchRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<AcademySuggestionDto>>> Handle(GetAcademySuggestionsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching academy suggestions for prefix: {Prefix}", request.Prefix);

        var academies = await _academySearchRepository.GetAutocompleteSuggestionsAsync(
            request.Prefix, request.Limit, cancellationToken);

        var suggestions = academies.Select(a => new AcademySuggestionDto
        {
            Id = a.Id,
            Name = a.Name,
            AcademyCode = a.AcademyCode,
            City = a.Contact?.City,
            State = a.Contact?.State,
            LogoUrl = a.LogoUrl,
            IsVerified = a.VerificationStatus == Domain.Enums.VerificationStatus.Verified
        }).ToList();

        return Result<IReadOnlyList<AcademySuggestionDto>>.Success(suggestions);
    }
}
