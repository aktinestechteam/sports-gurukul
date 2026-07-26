using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.NearbyAcademies;

public class NearbyAcademiesQuery : IRequest<Result<IReadOnlyList<AcademySearchResultDto>>>
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal RadiusKm { get; set; } = 10;
    public int Limit { get; set; } = 20;
    public string? SportName { get; set; }
}
