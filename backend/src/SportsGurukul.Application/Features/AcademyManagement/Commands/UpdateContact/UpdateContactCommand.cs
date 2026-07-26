using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateContact;

public class UpdateContactCommand : IRequest<Result<ContactDto>>
{
    public Guid AcademyId { get; set; }
    public string? PrimaryContactName { get; set; }
    public string? PrimaryPhone { get; set; }
    public string? PrimaryEmail { get; set; }
    public string? SecondaryContactName { get; set; }
    public string? SecondaryPhone { get; set; }
    public string? SecondaryEmail { get; set; }
    public string? Address { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}
