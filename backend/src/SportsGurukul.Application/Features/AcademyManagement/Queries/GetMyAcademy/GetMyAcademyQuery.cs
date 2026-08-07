using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetMyAcademy;

/// <summary>
/// Resolves the academy owned by the current user (their most recently
/// created academy), so admins can brand their dashboard.
/// </summary>
public class GetMyAcademyQuery : IRequest<Result<AcademyDto>>
{
    public Guid UserId { get; set; }
}
