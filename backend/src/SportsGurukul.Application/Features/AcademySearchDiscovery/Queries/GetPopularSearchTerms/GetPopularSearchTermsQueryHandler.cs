using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetPopularSearchTerms;

public class GetPopularSearchTermsQueryHandler : IRequestHandler<GetPopularSearchTermsQuery, Result<IReadOnlyList<string>>>
{
    private readonly IAcademySearchRepository _academySearchRepository;
    private readonly ILogger<GetPopularSearchTermsQueryHandler> _logger;

    public GetPopularSearchTermsQueryHandler(
        IAcademySearchRepository academySearchRepository,
        ILogger<GetPopularSearchTermsQueryHandler> logger)
    {
        _academySearchRepository = academySearchRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<string>>> Handle(GetPopularSearchTermsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching popular search terms, Limit={Limit}", request.Limit);

        var terms = await _academySearchRepository.GetPopularSearchTermsAsync(request.Limit, cancellationToken);

        return Result<IReadOnlyList<string>>.Success(terms);
    }
}
