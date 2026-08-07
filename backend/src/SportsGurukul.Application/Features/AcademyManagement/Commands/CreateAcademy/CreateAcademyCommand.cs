using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.CreateAcademy;

public class CreateAcademyCommand : IRequest<Result<AcademyDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? Description { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? GSTNumber { get; set; }
    public DateTime? EstablishedDate { get; set; }
    public string? Website { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public AcademyType AcademyType { get; set; } = AcademyType.MultiSport;

    /// <summary>Primary contact person's name.</summary>
    public string? PrimaryContactName { get; set; }

    /// <summary>Street address of the academy.</summary>
    public string? Address { get; set; }

    /// <summary>Country of the academy.</summary>
    public string? Country { get; set; }

    /// <summary>State or province of the academy.</summary>
    public string? State { get; set; }

    /// <summary>City of the academy.</summary>
    public string? City { get; set; }

    /// <summary>Postal code of the academy.</summary>
    public string? PostalCode { get; set; }

    /// <summary>Names of the sports offered by the academy. The first entry is
    /// treated as the academy's primary sport.</summary>
    public List<string> SportNames { get; set; } = [];

    /// <summary>Id of the user creating the academy. When set, the user is
    /// granted the <c>Academy Admin</c> role so they can manage the academy.</summary>
    public Guid? UserId { get; set; }
}
