using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.UserManagement.Commands.CreateUserProfile;

public class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, Result<UserProfileDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IRepository<ContactInformation> _contactRepository;
    private readonly IRepository<Address> _addressRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateUserProfileCommandHandler> _logger;

    public CreateUserProfileCommandHandler(
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        IRepository<ContactInformation> contactRepository,
        IRepository<Address> addressRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateUserProfileCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _contactRepository = contactRepository;
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<UserProfileDto>> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating profile for user: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            return Result<UserProfileDto>.Failure("User not found.");
        }

        var existingProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (existingProfile is not null && !existingProfile.IsDeleted)
        {
            _logger.LogWarning("Profile already exists for user: {UserId}", request.UserId);
            return Result<UserProfileDto>.Failure("A profile already exists for this user.");
        }

        UserProfile profile;
        if (existingProfile is not null && existingProfile.IsDeleted)
        {
            existingProfile.IsDeleted = false;
            existingProfile.DateOfBirth = request.DateOfBirth;
            existingProfile.Gender = request.Gender;
            existingProfile.Bio = request.Bio;
            existingProfile.Height = request.Height;
            existingProfile.Weight = request.Weight;
            existingProfile.PreferredSport = request.PreferredSport;
            existingProfile.ExperienceLevel = request.ExperienceLevel;
            existingProfile.UpdatedAt = DateTime.UtcNow;
            _userProfileRepository.Update(existingProfile);
            profile = existingProfile;
        }
        else
        {
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

        if (!string.IsNullOrWhiteSpace(request.PrimaryPhoneNumber))
        {
            var duplicatePhone = await _userRepository.GetByPhoneNumberAsync(request.PrimaryPhoneNumber, cancellationToken);
            if (duplicatePhone is not null && duplicatePhone.Id != request.UserId)
            {
                _logger.LogWarning("Phone number already in use: {Phone}", request.PrimaryPhoneNumber);
                return Result<UserProfileDto>.Failure("This phone number is already associated with another account.");
            }

            var contact = new ContactInformation
            {
                Id = Guid.NewGuid(),
                UserProfileId = profile.Id,
                PrimaryPhoneCountryCode = request.PrimaryPhoneCountryCode ?? "+91",
                PrimaryPhoneNumber = request.PrimaryPhoneNumber
            };
            await _contactRepository.AddAsync(contact, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(request.AddressLine1) && !string.IsNullOrWhiteSpace(request.City))
        {
            var address = new Address
            {
                Id = Guid.NewGuid(),
                UserProfileId = profile.Id,
                AddressType = request.AddressType,
                Line1 = request.AddressLine1,
                Line2 = request.AddressLine2,
                City = request.City,
                State = request.State ?? string.Empty,
                Country = request.Country ?? string.Empty,
                PostalCode = request.PostalCode,
                IsPrimary = true
            };
            await _addressRepository.AddAsync(address, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Profile created for user: {UserId}, ProfileId: {ProfileId}", request.UserId, profile.Id);

        var dto = MapToDto(profile, user);
        return Result<UserProfileDto>.Success(dto);
    }

    internal static UserProfileDto MapToDto(UserProfile profile, User user)
    {
        var dto = new UserProfileDto
        {
            Id = profile.Id,
            UserId = profile.UserId,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = profile.ContactInformation?.PrimaryPhoneNumber,
            DateOfBirth = profile.DateOfBirth,
            Gender = profile.Gender,
            Bio = profile.Bio,
            ProfileImageUrl = profile.ProfileImageUrl,
            CoverImageUrl = profile.CoverImageUrl,
            Height = profile.Height,
            Weight = profile.Weight,
            PreferredSport = profile.PreferredSport,
            ExperienceLevel = profile.ExperienceLevel,
            Status = user.Status,
            IsEmailVerified = user.IsEmailVerified,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt,
            Addresses = profile.Addresses.Where(a => !a.IsDeleted).Select(a => new AddressDto
            {
                Id = a.Id,
                AddressType = a.AddressType,
                Line1 = a.Line1,
                Line2 = a.Line2,
                City = a.City,
                State = a.State,
                Country = a.Country,
                PostalCode = a.PostalCode,
                IsPrimary = a.IsPrimary,
                Latitude = a.Latitude,
                Longitude = a.Longitude
            }).ToList(),
            ContactInformation = profile.ContactInformation is not null ? new ContactDto
            {
                Id = profile.ContactInformation.Id,
                PrimaryPhoneCountryCode = profile.ContactInformation.PrimaryPhoneCountryCode,
                PrimaryPhoneNumber = profile.ContactInformation.PrimaryPhoneNumber,
                PrimaryPhoneVerified = profile.ContactInformation.PrimaryPhoneVerified,
                SecondaryPhoneCountryCode = profile.ContactInformation.SecondaryPhoneCountryCode,
                SecondaryPhoneNumber = profile.ContactInformation.SecondaryPhoneNumber,
                SecondaryPhoneVerified = profile.ContactInformation.SecondaryPhoneVerified,
                WebsiteUrl = profile.ContactInformation.WebsiteUrl,
                FacebookUrl = profile.ContactInformation.FacebookUrl,
                TwitterUrl = profile.ContactInformation.TwitterUrl,
                InstagramUrl = profile.ContactInformation.InstagramUrl,
                LinkedInUrl = profile.ContactInformation.LinkedInUrl,
                YouTubeUrl = profile.ContactInformation.YouTubeUrl
            } : null,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
            Preferences = profile.UserPreference is not null ? new UserPreferenceDto
            {
                Id = profile.UserPreference.Id,
                Language = profile.UserPreference.Language,
                Theme = profile.UserPreference.Theme,
                TimeZone = profile.UserPreference.TimeZone,
                EmailNotifications = profile.UserPreference.EmailNotifications,
                PushNotifications = profile.UserPreference.PushNotifications,
                SmsNotifications = profile.UserPreference.SmsNotifications,
                MarketingEmails = profile.UserPreference.MarketingEmails,
                ProfileVisibility = profile.UserPreference.ProfileVisibility,
                ShowOnlineStatus = profile.UserPreference.ShowOnlineStatus
            } : null
        };

        dto.ProfileCompletionPercentage = CalculateCompletionPercentage(dto);
        return dto;
    }

    public static int CalculateCompletionPercentage(UserProfileDto profile)
    {
        int total = 8;
        int filled = 0;

        if (!string.IsNullOrWhiteSpace(profile.Bio)) filled++;
        if (profile.DateOfBirth.HasValue) filled++;
        if (profile.Gender != Gender.PreferNotToSay) filled++;
        if (!string.IsNullOrWhiteSpace(profile.Height)) filled++;
        if (!string.IsNullOrWhiteSpace(profile.Weight)) filled++;
        if (!string.IsNullOrWhiteSpace(profile.PreferredSport)) filled++;
        if (!string.IsNullOrWhiteSpace(profile.ExperienceLevel)) filled++;
        if (profile.ContactInformation is not null && !string.IsNullOrWhiteSpace(profile.ContactInformation.PrimaryPhoneNumber)) filled++;

        return (int)Math.Round((double)filled / total * 100);
    }
}
