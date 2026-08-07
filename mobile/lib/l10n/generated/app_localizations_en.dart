// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for English (`en`).
class AppLocalizationsEn extends AppLocalizations {
  AppLocalizationsEn([String locale = 'en']) : super(locale);

  @override
  String get appTitle => 'Sports Gurukul';

  @override
  String get appSplashTagline => 'Train • Compete • Excel';

  @override
  String get appInitializing => 'Initializing Sports Gurukul…';

  @override
  String get dashboardInitializedTitle => 'Project Initialized Successfully';

  @override
  String get dashboardInitializedMessage =>
      'The Sports Gurukul mobile foundation is ready. Features will be delivered in upcoming sprints.';

  @override
  String dashboardGreetingMorning(String name) {
    return 'Good morning, $name';
  }

  @override
  String dashboardGreetingAfternoon(String name) {
    return 'Good afternoon, $name';
  }

  @override
  String dashboardGreetingEvening(String name) {
    return 'Good evening, $name';
  }

  @override
  String get dashboardHeaderSubtitle => 'Your performance, one glance away.';

  @override
  String get dashboardStatActiveDays => 'Active days';

  @override
  String get dashboardStatAvgIntensity => 'Avg intensity';

  @override
  String get dashboardStatRecovery => 'Recovery';

  @override
  String get dashboardWeeklyTarget => 'Weekly target';

  @override
  String get dashboardWeeklyTargetDone =>
      '18 of 24 sessions completed this week';

  @override
  String get dashboardQuickActions => 'Quick actions';

  @override
  String get dashboardActionBookSession => 'Book session';

  @override
  String get dashboardActionFindCoach => 'Find coach';

  @override
  String get dashboardActionLeaderboards => 'Leaderboards';

  @override
  String get dashboardActionTournaments => 'Tournaments';

  @override
  String get dashboardTabOverview => 'Overview';

  @override
  String get dashboardTabTraining => 'Training';

  @override
  String get dashboardTabInsights => 'Insights';

  @override
  String get dashboardUpcomingSessions => 'Upcoming sessions';

  @override
  String get dashboardSessionEveningRun => 'Evening run';

  @override
  String get dashboardSessionEveningRunTime => 'Today · 6:00 PM';

  @override
  String get dashboardSessionSquadStrength => 'Squad strength';

  @override
  String get dashboardSessionSquadStrengthTime => 'Wed · 5:30 AM';

  @override
  String get dashboardInsightRecovery => 'Recovery score is up 12% this week';

  @override
  String get dashboardInsightIntensity => '4 days in the target intensity zone';

  @override
  String get dashboardInsightRestDay => 'Rest day optimized for Wednesday';

  @override
  String get authLoginTitle => 'Welcome!';

  @override
  String get authLoginSubtitle => 'Sign in to continue to Sports Gurukul.';

  @override
  String get authLoginEmailLabel => 'Email';

  @override
  String get authLoginPasswordLabel => 'Password';

  @override
  String get authLoginSubmit => 'Sign in';

  @override
  String get authLoginForgotPassword => 'Forgot password?';

  @override
  String get authLoginRememberMe => 'Remember Me';

  @override
  String get authLoginOr => 'OR';

  @override
  String get authLoginDontHaveAccount => 'Don\'t have an account?';

  @override
  String get authLoginSignUp => 'Sign Up';

  @override
  String get authSignUpTitle => 'Create your account';

  @override
  String get authSignUpSubtitle =>
      'Create a Sports Gurukul account to start your journey.';

  @override
  String get authSignUpNameLabel => 'Full name';

  @override
  String get authSignUpPhoneLabel => 'Phone number';

  @override
  String get authSignUpConfirmPasswordLabel => 'Confirm password';

  @override
  String get authSignUpSubmit => 'Create account';

  @override
  String get authSignUpAlreadyHaveAccount => 'Already have an account?';

  @override
  String get authSignUpSignIn => 'Sign In';

  @override
  String get authSignUpSuccess =>
      'Account created successfully. You are signed in.';

  @override
  String get authForgotTitle => 'Reset your password';

  @override
  String get authForgotSubtitle =>
      'Enter your email and we will send you a reset link.';

  @override
  String get authForgotSubmit => 'Send reset link';

  @override
  String get authForgotEmailSent =>
      'If an account with that email exists, a reset link has been sent.';

  @override
  String get authForgotBackToLogin => 'Back to sign in';

  @override
  String get authResetTitle => 'Choose a new password';

  @override
  String get authResetSubtitle =>
      'Enter a strong password you have not used before.';

  @override
  String get authResetNewPasswordLabel => 'New password';

  @override
  String get authResetConfirmPasswordLabel => 'Confirm new password';

  @override
  String get authResetSubmit => 'Reset password';

  @override
  String get authResetSuccess =>
      'Password reset successfully. Please sign in with your new password.';

  @override
  String get authResetMissingToken =>
      'This reset link is missing a token. Please open it from your email.';

  @override
  String get authShowPassword => 'Show password';

  @override
  String get authHidePassword => 'Hide password';

  @override
  String get authLogout => 'Log out';

  @override
  String get authErrorsInvalidCredentials => 'Invalid email or password.';

  @override
  String get authErrorsAccountLocked =>
      'Account locked due to too many failed attempts. Try again in 15 minutes.';

  @override
  String get authErrorsSessionExpired =>
      'Your session has expired. Please sign in again.';

  @override
  String get authErrorsBadRequest =>
      'The request could not be processed. Please check your details.';

  @override
  String get authErrorsNetwork =>
      'You seem to be offline. Check your connection and try again.';

  @override
  String get authErrorsRateLimited =>
      'Too many attempts. Please try again later.';

  @override
  String get authErrorsServer =>
      'Something went wrong on our side. Please try again later.';

  @override
  String get authErrorsUnknown => 'Something went wrong. Please try again.';

  @override
  String get validationRequired => 'This field is required.';

  @override
  String get validationEmailInvalid => 'Enter a valid email address.';

  @override
  String validationPasswordTooShort(int min) {
    return 'Password must be at least $min characters.';
  }

  @override
  String validationPasswordTooLong(int max) {
    return 'Password must be at most $max characters.';
  }

  @override
  String get validationPasswordUppercase =>
      'Password must contain at least one uppercase letter.';

  @override
  String get validationPasswordLowercase =>
      'Password must contain at least one lowercase letter.';

  @override
  String get validationPasswordDigit =>
      'Password must contain at least one number.';

  @override
  String get validationPasswordSpecial =>
      'Password must contain at least one special character.';

  @override
  String get validationPasswordMismatch => 'Passwords do not match.';

  @override
  String get validationPhoneInvalid => 'Enter a valid 10-digit mobile number.';

  @override
  String get validationDateInvalid => 'Enter a valid date.';

  @override
  String get validationDateFuture => 'Date of birth cannot be in the future.';

  @override
  String get validationNumberInvalid => 'Enter a valid number.';

  @override
  String get validationPostalCodeInvalid => 'Enter a valid postal code.';

  @override
  String get profileMyProfileTitle => 'My Profile';

  @override
  String get profileRetry => 'Retry';

  @override
  String get profileErrorTitle => 'Could not load profile';

  @override
  String get profileFullName => 'Full name';

  @override
  String get profileEmail => 'Email';

  @override
  String get profilePhone => 'Phone';

  @override
  String get profilePreferredSport => 'Preferred sport';

  @override
  String get profileExperienceLevel => 'Experience level';

  @override
  String get profileHeight => 'Height';

  @override
  String get profileWeight => 'Weight';

  @override
  String get profileEditProfile => 'Edit profile';

  @override
  String get profileCreateProfile => 'Create profile';

  @override
  String get profileCreateTitle => 'Create profile';

  @override
  String get profileCreateSubtitle => 'Tell us a little about yourself.';

  @override
  String get profileCreatePrompt => 'Create your profile to get started.';

  @override
  String get profileAboutMe => 'About me';

  @override
  String get profileAddresses => 'Addresses';

  @override
  String get profileEditTitle => 'Edit profile';

  @override
  String get profileEditSubtitle => 'Keep your details up to date.';

  @override
  String get profileEditBasicInfo => 'Basic information';

  @override
  String get profileGender => 'Gender';

  @override
  String get profileGenderMale => 'Male';

  @override
  String get profileGenderFemale => 'Female';

  @override
  String get profileGenderNonBinary => 'Non-binary';

  @override
  String get profileGenderPreferNotToSay => 'Prefer not to say';

  @override
  String get profileBio => 'Bio';

  @override
  String get profileEditContactInfo => 'Contact information';

  @override
  String get profileCountryCode => 'Code';

  @override
  String get profileEditAddress => 'Address';

  @override
  String get profileAddressType => 'Address type';

  @override
  String get profileAddressTypeHome => 'Home';

  @override
  String get profileAddressTypeWork => 'Work';

  @override
  String get profileAddressTypeAcademy => 'Academy';

  @override
  String get profileAddressTypeOther => 'Other';

  @override
  String get profileAddressLine1 => 'Address line 1';

  @override
  String get profileAddressLine2 => 'Address line 2';

  @override
  String get profileCity => 'City';

  @override
  String get profileState => 'State';

  @override
  String get profileCountry => 'Country';

  @override
  String get profilePostalCode => 'Postal code';

  @override
  String get profileDateOfBirth => 'Date of birth';

  @override
  String get profileSave => 'Save';

  @override
  String get profileUpdateSuccess => 'Profile updated successfully.';

  @override
  String get profilePreferencesTitle => 'Preferences';

  @override
  String get profilePreferencesSubtitle =>
      'Customize notifications, appearance and privacy.';

  @override
  String get profilePreferencesAppearance => 'Appearance';

  @override
  String get profilePreferencesTheme => 'Theme';

  @override
  String get profileThemeLight => 'Light';

  @override
  String get profileThemeDark => 'Dark';

  @override
  String get profileThemeSystem => 'System';

  @override
  String get profilePreferencesNotifications => 'Notifications';

  @override
  String get profilePreferenceEmailNotifications => 'Email notifications';

  @override
  String get profilePreferencePushNotifications => 'Push notifications';

  @override
  String get profilePreferenceSmsNotifications => 'SMS notifications';

  @override
  String get profilePreferenceMarketingEmails => 'Marketing emails';

  @override
  String get profilePreferencesPrivacy => 'Privacy';

  @override
  String get profilePreferenceProfileVisibility => 'Profile visibility';

  @override
  String get profilePreferenceShowOnlineStatus => 'Show online status';

  @override
  String get profilePreferencesSaved => 'Preferences saved successfully.';

  @override
  String get profileChangePhoto => 'Change photo';

  @override
  String get profilePhotoUploaded => 'Profile photo uploaded.';

  @override
  String get profilePhotoRemoved => 'Profile photo removed.';

  @override
  String get profileRemovePhoto => 'Remove photo';

  @override
  String get profileErrorsNotFound => 'Profile not found.';

  @override
  String get profileErrorsNetwork =>
      'You seem to be offline. Check your connection and try again.';

  @override
  String get profileErrorsServer =>
      'Something went wrong on our side. Please try again later.';

  @override
  String get profileErrorsValidation =>
      'The request could not be processed. Please check your details.';

  @override
  String get profileErrorsPhotoUpload =>
      'The photo could not be uploaded. Please try a JPEG, PNG or WebP image under 5 MB.';

  @override
  String get profileErrorsPhotoNotFound => 'No profile photo found.';

  @override
  String get profileErrorsPermission =>
      'You do not have permission to perform this action.';

  @override
  String get profileErrorsUnknown => 'Something went wrong. Please try again.';

  @override
  String get welcomeTitle => 'Welcome to Sports Gurukul!';

  @override
  String get welcomeSubtitle =>
      'You\'re all set. Pick a path to kick-start your sporting journey.';

  @override
  String get welcomeCreateAcademy => 'Create My Academy';

  @override
  String get welcomeCreateAcademySubtitle =>
      'Set up a new academy and start managing athletes.';

  @override
  String get welcomeJoinAcademy => 'Join Existing Academy';

  @override
  String get welcomeJoinAcademySubtitle =>
      'Find an academy and join as a member.';

  @override
  String get welcomeExplore => 'Explore Application';

  @override
  String get welcomeExploreSubtitle =>
      'Skip the setup and browse the dashboard for now.';

  @override
  String get welcomeLoading => 'Preparing your experience…';

  @override
  String get welcomeEmptyMessage => 'Nothing to resolve yet.';

  @override
  String get welcomeErrorTitle => 'We couldn\'t load your account';

  @override
  String get welcomeErrorsNetwork =>
      'You seem to be offline. Check your connection and try again.';

  @override
  String get welcomeErrorsServer =>
      'Something went wrong on our side. Please try again later.';

  @override
  String get welcomeErrorsSessionExpired =>
      'Your session has expired. Please sign in again.';

  @override
  String get welcomeErrorsUnknown => 'Something went wrong. Please try again.';

  @override
  String get welcomeRetry => 'Try again';

  @override
  String get academyCreateTitle => 'Create Academy';

  @override
  String get academyCreateMessage =>
      'Academy setup is coming in a later sprint. You can explore the application in the meantime.';

  @override
  String get academyJoinTitle => 'Join an Academy';

  @override
  String get academyJoinMessage =>
      'Finding and joining an academy is coming in a later sprint. You can explore the application in the meantime.';

  @override
  String get academyBackToDashboard => 'Back to dashboard';

  @override
  String get academyCreateSubtitle =>
      'Tell us about your academy and set it up in a few steps.';

  @override
  String academyStepIndicator(int current, int total) {
    return 'Step $current of $total';
  }

  @override
  String get academyStepBasics => 'Basic information';

  @override
  String get academyStepContact => 'Contact information';

  @override
  String get academyStepAddress => 'Address';

  @override
  String get academyStepBranding => 'Branding';

  @override
  String get academyStepReview => 'Review & submit';

  @override
  String get academyNameLabel => 'Academy name';

  @override
  String get academyDescriptionLabel => 'Description';

  @override
  String get academyTypeLabel => 'Academy type';

  @override
  String get academyTypeSingleSport => 'Single-sport';

  @override
  String get academyTypeSingleSportHint => 'Focus on one sport.';

  @override
  String get academyTypeMultiSport => 'Multi-sport';

  @override
  String get academyTypeMultiSportHint => 'Offer several sports.';

  @override
  String get academySportsLabel => 'Sports offered';

  @override
  String get academySelectAtLeastOneSport => 'Select at least one sport.';

  @override
  String get academyContactPersonLabel => 'Contact person';

  @override
  String get academyEmailLabel => 'Academy email';

  @override
  String get academyPhoneLabel => 'Mobile number';

  @override
  String get academyWebsiteLabel => 'Website (optional)';

  @override
  String get academyCountryLabel => 'Country';

  @override
  String get academyStateLabel => 'State';

  @override
  String get academyCityLabel => 'City';

  @override
  String get academyAddressLineLabel => 'Address line';

  @override
  String get academyPostalCodeLabel => 'Postal code (optional)';

  @override
  String get academyLogoLabel => 'Academy logo';

  @override
  String get academyLogoRequired => 'Please add an academy logo.';

  @override
  String get academyLogoHint =>
      'A clear square logo (JPEG, PNG or WebP, under 5 MB).';

  @override
  String get academyCoverLabel => 'Cover image';

  @override
  String get academyCoverHint => 'Optional. A wide banner image looks best.';

  @override
  String get academyChooseImage => 'Choose from gallery';

  @override
  String get academyReplaceImage => 'Replace';

  @override
  String get academyRemoveImage => 'Remove';

  @override
  String get academyReviewTitle => 'Review & submit';

  @override
  String get academyReviewSubtitle =>
      'Check everything looks right before creating your academy.';

  @override
  String get academyReviewEdit => 'Edit';

  @override
  String get academyReviewNotProvided => 'Not provided';

  @override
  String get academyBackButton => 'Back';

  @override
  String get academyNextButton => 'Continue';

  @override
  String get academySubmitButton => 'Create academy';

  @override
  String get academyEditTitle => 'Edit Academy';

  @override
  String get academyEditSubtitle =>
      'Update your academy details and save the changes.';

  @override
  String get academyEditSaveButton => 'Save Changes';

  @override
  String get academyTypeSportsLocked =>
      'Academy type and sports cannot be changed after creation.';

  @override
  String academySubmitSuccess(String name) {
    return '$name has been created. Welcome to Sports Gurukul!';
  }

  @override
  String get academyErrorsNetwork =>
      'You seem to be offline. Check your connection and try again.';

  @override
  String get academyErrorsServer =>
      'Something went wrong on our side. Please try again later.';

  @override
  String get academyErrorsValidation =>
      'The request could not be processed. Please check your details.';

  @override
  String get academyErrorsPermission =>
      'You do not have permission to create an academy.';

  @override
  String get academyErrorsUnknown => 'Something went wrong. Please try again.';

  @override
  String get validationUrlInvalid => 'Enter a valid URL.';

  @override
  String get roleLabelPlatformAdministrator => 'Platform Administrator';

  @override
  String get roleLabelAcademy => 'Academy';

  @override
  String get roleLabelCoach => 'Coach';

  @override
  String get roleLabelAthlete => 'Athlete';

  @override
  String get roleLabelParent => 'Parent';

  @override
  String get roleLabelScout => 'Scout';

  @override
  String get roleLabelSponsor => 'Sponsor';

  @override
  String get roleLabelAiAdministrator => 'AI Administrator';

  @override
  String get roleLabelMember => 'Member';

  @override
  String get roleLabelNewUser => 'New User';

  @override
  String get roleLabelRegisteredUser => 'Registered User';

  @override
  String get roleLabelAcademyAdmin => 'Academy Admin';

  @override
  String get roleLabelPendingApproval => 'Pending Approval';

  @override
  String get roleLabelSystemAdmin => 'System Admin';
}
