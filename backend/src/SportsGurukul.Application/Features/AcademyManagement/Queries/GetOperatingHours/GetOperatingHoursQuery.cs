using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetOperatingHours;

public class GetOperatingHoursQuery : IRequest<Result<OperatingHoursDto>>
{
    public Guid AcademyId { get; set; }
}
