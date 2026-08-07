using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.UserManagement.Commands.CreateUserProfile;
using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.UserManagement.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Result<UserProfileDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IRepository<Domain.Entities.ContactInformation> _contactRepository;
    private readonly IRepository<Domain.Entities.Address> _addressRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateUserProfileCommandHandler> _logger;

    public UpdateUserProfileCommandHandler(
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        IRepository<Domain.Entities.ContactInformation> contactRepository,
        IRepository<Domain.Entities.Address> addressRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateUserProfileCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _contactRepository = contactRepository;
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<UserProfileDto>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating profile for user: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            return Result<UserProfileDto>.Failure("User not found.");
        }

        var profile = await _userProfileRepository.GetFullProfileAsync(request.UserId, cancellationToken);
        var isNewProfile = profile is null;
        if (profile is null)
        {
            _logger.LogInformation("Profile not found for user, creating via upsert: {UserId}", request.UserId);
            profile = new UserProfile
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Bio = request.Bio,
                Height = request.Height,
                Weight = request.Weight,
                PreferredSport = request.PreferredSport,
                ExperienceLevel = request.ExperienceLevel
            };
            await _userProfileRepository.AddAsync(profile, cancellationToken);
        }
        else if (profile.IsDeleted)
        {
            _logger.LogWarning("Profile is deleted for user: {UserId}", request.UserId);
            return Result<UserProfileDto>.Failure("Profile has been deleted. Please restore it first.");
        }

        profile.DateOfBirth = request.DateOfBirth ?? profile.DateOfBirth;
        profile.Gender = request.Gender;
        profile.Bio = request.Bio ?? profile.Bio;
        profile.Height = request.Height ?? profile.Height;
        profile.Weight = request.Weight ?? profile.Weight;
        profile.PreferredSport = request.PreferredSport ?? profile.PreferredSport;
        profile.ExperienceLevel = request.ExperienceLevel ?? profile.ExperienceLevel;
        profile.UpdatedAt = DateTime.UtcNow;

        if (!isNewProfile)
        {
            _userProfileRepository.Update(profile);
        }

        if (!string.IsNullOrWhiteSpace(request.PrimaryPhoneNumber))
        {
            var existingContact = profile.ContactInformation;
            if (existingContact is null)
            {
                existingContact = new Domain.Entities.ContactInformation
                {
                    Id = Guid.NewGuid(),
                    UserProfileId = profile.Id,
                    PrimaryPhoneCountryCode = request.PrimaryPhoneCountryCode ?? "+91",
                    PrimaryPhoneNumber = request.PrimaryPhoneNumber
                };
                await _contactRepository.AddAsync(existingContact, cancellationToken);
            }
            else if (existingContact.PrimaryPhoneNumber != request.PrimaryPhoneNumber)
            {
                var duplicatePhone = await _userRepository.GetByPhoneNumberAsync(request.PrimaryPhoneNumber, cancellationToken);
                if (duplicatePhone is not null && duplicatePhone.Id != request.UserId)
                {
                    _logger.LogWarning("Phone number already in use: {Phone}", request.PrimaryPhoneNumber);
                    return Result<UserProfileDto>.Failure("This phone number is already associated with another account.");
                }

                existingContact.PrimaryPhoneCountryCode = request.PrimaryPhoneCountryCode ?? existingContact.PrimaryPhoneCountryCode;
                existingContact.PrimaryPhoneNumber = request.PrimaryPhoneNumber;
                existingContact.UpdatedAt = DateTime.UtcNow;
                _contactRepository.Update(existingContact);
            }
        }

        if (!string.IsNullOrWhiteSpace(request.AddressLine1) && !string.IsNullOrWhiteSpace(request.City))
        {
            var primaryAddress = profile.Addresses.FirstOrDefault(a => a.IsPrimary && !a.IsDeleted);
            if (primaryAddress is null)
            {
                primaryAddress = new Domain.Entities.Address
                {
                    Id = Guid.NewGuid(),
                    UserProfileId = profile.Id,
                    AddressType = request.AddressType ?? Domain.Enums.AddressType.Home,
                    Line1 = request.AddressLine1,
                    Line2 = request.AddressLine2,
                    City = request.City,
                    State = request.State ?? string.Empty,
                    Country = request.Country ?? string.Empty,
                    PostalCode = request.PostalCode,
                    IsPrimary = true
                };
                await _addressRepository.AddAsync(primaryAddress, cancellationToken);
            }
            else
            {
                primaryAddress.AddressType = request.AddressType ?? primaryAddress.AddressType;
                primaryAddress.Line1 = request.AddressLine1;
                primaryAddress.Line2 = request.AddressLine2 ?? primaryAddress.Line2;
                primaryAddress.City = request.City;
                primaryAddress.State = request.State ?? primaryAddress.State;
                primaryAddress.Country = request.Country ?? primaryAddress.Country;
                primaryAddress.PostalCode = request.PostalCode ?? primaryAddress.PostalCode;
                primaryAddress.UpdatedAt = DateTime.UtcNow;
                _addressRepository.Update(primaryAddress);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Profile updated for user: {UserId}", request.UserId);

        var updatedProfile = await _userProfileRepository.GetFullProfileAsync(request.UserId, cancellationToken);
        var dto = CreateUserProfileCommandHandler.MapToDto(updatedProfile!, user);
        return Result<UserProfileDto>.Success(dto);
    }
}
