using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.UserManagement.Commands.UpdateUserProfile;

public class UpdateUserProfileCommand : IRequest<Result<UserProfileDto>>
{
    public Guid UserId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender Gender { get; set; } = Gender.PreferNotToSay;
    public string? Bio { get; set; }
    public string? Height { get; set; }
    public string? Weight { get; set; }
    public string? PreferredSport { get; set; }
    public string? ExperienceLevel { get; set; }
    public string? PrimaryPhoneCountryCode { get; set; }
    public string? PrimaryPhoneNumber { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public AddressType? AddressType { get; set; }
}
