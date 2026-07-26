using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.UpdatePricing;

public class UpdatePricingCommand : IRequest<Result<PricingDto>>
{
    public Guid FacilityId { get; set; }
    public string PricingName { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal MonthlyRate { get; set; }
    public decimal? PeakHourlyRate { get; set; }
    public decimal? OffPeakHourlyRate { get; set; }
    public string? Description { get; set; }
}
