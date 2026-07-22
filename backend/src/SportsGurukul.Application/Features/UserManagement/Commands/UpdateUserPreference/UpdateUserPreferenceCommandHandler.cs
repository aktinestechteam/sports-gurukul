using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.UserManagement.Commands.UpdateUserPreference;

public class UpdateUserPreferenceCommandHandler : IRequestHandler<UpdateUserPreferenceCommand, Result<UserPreferenceDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IRepository<UserPreference> _preferenceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateUserPreferenceCommandHandler> _logger;

    public UpdateUserPreferenceCommandHandler(
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        IRepository<UserPreference> preferenceRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateUserPreferenceCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _preferenceRepository = preferenceRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<UserPreferenceDto>> Handle(UpdateUserPreferenceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating preferences for user: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            return Result<UserPreferenceDto>.Failure("User not found.");
        }

        var profile = await _userProfileRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (profile is null || profile.IsDeleted)
        {
            _logger.LogWarning("Profile not found for user: {UserId}", request.UserId);
            return Result<UserPreferenceDto>.Failure("Profile not found. Please create a profile first.");
        }

        var preference = profile.UserPreference;
        if (preference is null)
        {
            preference = new UserPreference
            {
                Id = Guid.NewGuid(),
                UserProfileId = profile.Id
            };
            await _preferenceRepository.AddAsync(preference, cancellationToken);
        }

        if (request.Language is not null) preference.Language = request.Language;
        if (request.Theme.HasValue) preference.Theme = request.Theme.Value;
        if (request.TimeZone is not null) preference.TimeZone = request.TimeZone;
        if (request.EmailNotifications.HasValue) preference.EmailNotifications = request.EmailNotifications.Value;
        if (request.PushNotifications.HasValue) preference.PushNotifications = request.PushNotifications.Value;
        if (request.SmsNotifications.HasValue) preference.SmsNotifications = request.SmsNotifications.Value;
        if (request.MarketingEmails.HasValue) preference.MarketingEmails = request.MarketingEmails.Value;
        if (request.ProfileVisibility.HasValue) preference.ProfileVisibility = request.ProfileVisibility.Value;
        if (request.ShowOnlineStatus.HasValue) preference.ShowOnlineStatus = request.ShowOnlineStatus.Value;
        preference.UpdatedAt = DateTime.UtcNow;

        _preferenceRepository.Update(preference);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Preferences updated for user: {UserId}", request.UserId);

        var dto = new UserPreferenceDto
        {
            Id = preference.Id,
            Language = preference.Language,
            Theme = preference.Theme,
            TimeZone = preference.TimeZone,
            EmailNotifications = preference.EmailNotifications,
            PushNotifications = preference.PushNotifications,
            SmsNotifications = preference.SmsNotifications,
            MarketingEmails = preference.MarketingEmails,
            ProfileVisibility = preference.ProfileVisibility,
            ShowOnlineStatus = preference.ShowOnlineStatus
        };

        return Result<UserPreferenceDto>.Success(dto);
    }
}
