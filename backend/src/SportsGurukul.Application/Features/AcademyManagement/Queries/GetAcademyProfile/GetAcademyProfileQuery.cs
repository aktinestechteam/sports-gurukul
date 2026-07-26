using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetAcademyProfile;

public class GetAcademyProfileQuery : IRequest<Result<AcademyDto>>
{
    public string AcademyCode { get; set; } = string.Empty;
}
