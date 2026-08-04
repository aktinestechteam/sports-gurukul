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
}
