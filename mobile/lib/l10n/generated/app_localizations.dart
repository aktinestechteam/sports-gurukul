import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:intl/intl.dart' as intl;

import 'app_localizations_en.dart';
import 'app_localizations_hi.dart';
import 'app_localizations_mr.dart';

// ignore_for_file: type=lint

/// Callers can lookup localized strings with an instance of AppLocalizations
/// returned by `AppLocalizations.of(context)`.
///
/// Applications need to include `AppLocalizations.delegate()` in their app's
/// `localizationDelegates` list, and the locales they support in the app's
/// `supportedLocales` list. For example:
///
/// ```dart
/// import 'generated/app_localizations.dart';
///
/// return MaterialApp(
///   localizationsDelegates: AppLocalizations.localizationsDelegates,
///   supportedLocales: AppLocalizations.supportedLocales,
///   home: MyApplicationHome(),
/// );
/// ```
///
/// ## Update pubspec.yaml
///
/// Please make sure to update your pubspec.yaml to include the following
/// packages:
///
/// ```yaml
/// dependencies:
///   # Internationalization support.
///   flutter_localizations:
///     sdk: flutter
///   intl: any # Use the pinned version from flutter_localizations
///
///   # Rest of dependencies
/// ```
///
/// ## iOS Applications
///
/// iOS applications define key application metadata, including supported
/// locales, in an Info.plist file that is built into the application bundle.
/// To configure the locales supported by your app, you’ll need to edit this
/// file.
///
/// First, open your project’s ios/Runner.xcworkspace Xcode workspace file.
/// Then, in the Project Navigator, open the Info.plist file under the Runner
/// project’s Runner folder.
///
/// Next, select the Information Property List item, select Add Item from the
/// Editor menu, then select Localizations from the pop-up menu.
///
/// Select and expand the newly-created Localizations item then, for each
/// locale your application supports, add a new item and select the locale
/// you wish to add from the pop-up menu in the Value field. This list should
/// be consistent with the languages listed in the AppLocalizations.supportedLocales
/// property.
abstract class AppLocalizations {
  AppLocalizations(String locale)
    : localeName = intl.Intl.canonicalizedLocale(locale.toString());

  final String localeName;

  static AppLocalizations of(BuildContext context) {
    return Localizations.of<AppLocalizations>(context, AppLocalizations)!;
  }

  static const LocalizationsDelegate<AppLocalizations> delegate =
      _AppLocalizationsDelegate();

  /// A list of this localizations delegate along with the default localizations
  /// delegates.
  ///
  /// Returns a list of localizations delegates containing this delegate along with
  /// GlobalMaterialLocalizations.delegate, GlobalCupertinoLocalizations.delegate,
  /// and GlobalWidgetsLocalizations.delegate.
  ///
  /// Additional delegates can be added by appending to this list in
  /// MaterialApp. This list does not have to be used at all if a custom list
  /// of delegates is preferred or required.
  static const List<LocalizationsDelegate<dynamic>> localizationsDelegates =
      <LocalizationsDelegate<dynamic>>[
        delegate,
        GlobalMaterialLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
      ];

  /// A list of this localizations delegate's supported locales.
  static const List<Locale> supportedLocales = <Locale>[
    Locale('en'),
    Locale('hi'),
    Locale('mr'),
  ];

  /// The application name displayed in the app bar and launcher.
  ///
  /// In en, this message translates to:
  /// **'Sports Gurukul'**
  String get appTitle;

  /// Tagline shown on the splash screen.
  ///
  /// In en, this message translates to:
  /// **'Train • Compete • Excel'**
  String get appSplashTagline;

  /// Status text shown on the splash screen while the app initializes.
  ///
  /// In en, this message translates to:
  /// **'Initializing Sports Gurukul…'**
  String get appInitializing;

  /// Placeholder dashboard heading shown after successful project bootstrap.
  ///
  /// In en, this message translates to:
  /// **'Project Initialized Successfully'**
  String get dashboardInitializedTitle;

  /// Placeholder dashboard body text describing the current project state.
  ///
  /// In en, this message translates to:
  /// **'The Sports Gurukul mobile foundation is ready. Features will be delivered in upcoming sprints.'**
  String get dashboardInitializedMessage;

  /// Dashboard greeting shown before noon.
  ///
  /// In en, this message translates to:
  /// **'Good morning, {name}'**
  String dashboardGreetingMorning(String name);

  /// Dashboard greeting shown between noon and evening.
  ///
  /// In en, this message translates to:
  /// **'Good afternoon, {name}'**
  String dashboardGreetingAfternoon(String name);

  /// Dashboard greeting shown in the evening.
  ///
  /// In en, this message translates to:
  /// **'Good evening, {name}'**
  String dashboardGreetingEvening(String name);

  /// Subtitle under the dashboard greeting.
  ///
  /// In en, this message translates to:
  /// **'Your performance, one glance away.'**
  String get dashboardHeaderSubtitle;

  /// Label of the active-days stat card.
  ///
  /// In en, this message translates to:
  /// **'Active days'**
  String get dashboardStatActiveDays;

  /// Label of the average-intensity stat card.
  ///
  /// In en, this message translates to:
  /// **'Avg intensity'**
  String get dashboardStatAvgIntensity;

  /// Label of the recovery stat card.
  ///
  /// In en, this message translates to:
  /// **'Recovery'**
  String get dashboardStatRecovery;

  /// Title of the weekly-target progress card.
  ///
  /// In en, this message translates to:
  /// **'Weekly target'**
  String get dashboardWeeklyTarget;

  /// Supporting text under the weekly-target progress.
  ///
  /// In en, this message translates to:
  /// **'18 of 24 sessions completed this week'**
  String get dashboardWeeklyTargetDone;

  /// Section title for the quick-action cards.
  ///
  /// In en, this message translates to:
  /// **'Quick actions'**
  String get dashboardQuickActions;

  /// Quick action that starts session booking.
  ///
  /// In en, this message translates to:
  /// **'Book session'**
  String get dashboardActionBookSession;

  /// Quick action that searches for a coach.
  ///
  /// In en, this message translates to:
  /// **'Find coach'**
  String get dashboardActionFindCoach;

  /// Quick action that opens the leaderboards.
  ///
  /// In en, this message translates to:
  /// **'Leaderboards'**
  String get dashboardActionLeaderboards;

  /// Quick action that lists tournaments.
  ///
  /// In en, this message translates to:
  /// **'Tournaments'**
  String get dashboardActionTournaments;

  /// Dashboard tab showing a weekly snapshot.
  ///
  /// In en, this message translates to:
  /// **'Overview'**
  String get dashboardTabOverview;

  /// Dashboard tab showing upcoming training sessions.
  ///
  /// In en, this message translates to:
  /// **'Training'**
  String get dashboardTabTraining;

  /// Dashboard tab showing performance insights.
  ///
  /// In en, this message translates to:
  /// **'Insights'**
  String get dashboardTabInsights;

  /// Title of the upcoming-sessions list.
  ///
  /// In en, this message translates to:
  /// **'Upcoming sessions'**
  String get dashboardUpcomingSessions;

  /// Name of an upcoming training session.
  ///
  /// In en, this message translates to:
  /// **'Evening run'**
  String get dashboardSessionEveningRun;

  /// Date and time of the evening-run session.
  ///
  /// In en, this message translates to:
  /// **'Today · 6:00 PM'**
  String get dashboardSessionEveningRunTime;

  /// Name of an upcoming training session.
  ///
  /// In en, this message translates to:
  /// **'Squad strength'**
  String get dashboardSessionSquadStrength;

  /// Date and time of the squad-strength session.
  ///
  /// In en, this message translates to:
  /// **'Wed · 5:30 AM'**
  String get dashboardSessionSquadStrengthTime;

  /// Insight card describing an improving recovery score.
  ///
  /// In en, this message translates to:
  /// **'Recovery score is up 12% this week'**
  String get dashboardInsightRecovery;

  /// Insight card describing intensity-zone adherence.
  ///
  /// In en, this message translates to:
  /// **'4 days in the target intensity zone'**
  String get dashboardInsightIntensity;

  /// Insight card recommending a rest day.
  ///
  /// In en, this message translates to:
  /// **'Rest day optimized for Wednesday'**
  String get dashboardInsightRestDay;

  /// Heading of the login screen.
  ///
  /// In en, this message translates to:
  /// **'Welcome!'**
  String get authLoginTitle;

  /// Supporting text under the login heading.
  ///
  /// In en, this message translates to:
  /// **'Sign in to continue to Sports Gurukul.'**
  String get authLoginSubtitle;

  /// Label of the email field on login and forgot-password screens.
  ///
  /// In en, this message translates to:
  /// **'Email'**
  String get authLoginEmailLabel;

  /// Label of the password field on the login screen.
  ///
  /// In en, this message translates to:
  /// **'Password'**
  String get authLoginPasswordLabel;

  /// Label of the login submit button.
  ///
  /// In en, this message translates to:
  /// **'Sign in'**
  String get authLoginSubmit;

  /// Link to the forgot-password screen.
  ///
  /// In en, this message translates to:
  /// **'Forgot password?'**
  String get authLoginForgotPassword;

  /// Label of the remember-me checkbox on the login screen.
  ///
  /// In en, this message translates to:
  /// **'Remember Me'**
  String get authLoginRememberMe;

  /// Divider label between the primary login action and alternative sign-in options.
  ///
  /// In en, this message translates to:
  /// **'OR'**
  String get authLoginOr;

  /// Prompt above the sign-up link on the login screen.
  ///
  /// In en, this message translates to:
  /// **'Don\'t have an account?'**
  String get authLoginDontHaveAccount;

  /// Link to account creation shown on the login screen.
  ///
  /// In en, this message translates to:
  /// **'Sign Up'**
  String get authLoginSignUp;

  /// Heading of the sign-up screen.
  ///
  /// In en, this message translates to:
  /// **'Create your account'**
  String get authSignUpTitle;

  /// Supporting text under the sign-up heading.
  ///
  /// In en, this message translates to:
  /// **'Create a Sports Gurukul account to start your journey.'**
  String get authSignUpSubtitle;

  /// Label of the full-name field on the sign-up screen.
  ///
  /// In en, this message translates to:
  /// **'Full name'**
  String get authSignUpNameLabel;

  /// Label of the optional phone-number field on the sign-up screen.
  ///
  /// In en, this message translates to:
  /// **'Phone number'**
  String get authSignUpPhoneLabel;

  /// Label of the confirm-password field on the sign-up screen.
  ///
  /// In en, this message translates to:
  /// **'Confirm password'**
  String get authSignUpConfirmPasswordLabel;

  /// Label of the sign-up submit button.
  ///
  /// In en, this message translates to:
  /// **'Create account'**
  String get authSignUpSubmit;

  /// Prompt above the sign-in link on the sign-up screen.
  ///
  /// In en, this message translates to:
  /// **'Already have an account?'**
  String get authSignUpAlreadyHaveAccount;

  /// Link to the login screen shown on the sign-up screen.
  ///
  /// In en, this message translates to:
  /// **'Sign In'**
  String get authSignUpSignIn;

  /// Confirmation shown after a successful registration.
  ///
  /// In en, this message translates to:
  /// **'Account created successfully. You are signed in.'**
  String get authSignUpSuccess;

  /// Heading of the forgot-password screen.
  ///
  /// In en, this message translates to:
  /// **'Reset your password'**
  String get authForgotTitle;

  /// Supporting text under the forgot-password heading.
  ///
  /// In en, this message translates to:
  /// **'Enter your email and we will send you a reset link.'**
  String get authForgotSubtitle;

  /// Label of the forgot-password submit button.
  ///
  /// In en, this message translates to:
  /// **'Send reset link'**
  String get authForgotSubmit;

  /// Confirmation shown after requesting a password reset email.
  ///
  /// In en, this message translates to:
  /// **'If an account with that email exists, a reset link has been sent.'**
  String get authForgotEmailSent;

  /// Link back to the login screen from the forgot/reset screens.
  ///
  /// In en, this message translates to:
  /// **'Back to sign in'**
  String get authForgotBackToLogin;

  /// Heading of the reset-password screen.
  ///
  /// In en, this message translates to:
  /// **'Choose a new password'**
  String get authResetTitle;

  /// Supporting text under the reset-password heading.
  ///
  /// In en, this message translates to:
  /// **'Enter a strong password you have not used before.'**
  String get authResetSubtitle;

  /// Label of the new-password field on the reset screen.
  ///
  /// In en, this message translates to:
  /// **'New password'**
  String get authResetNewPasswordLabel;

  /// Label of the confirm-password field on the reset screen.
  ///
  /// In en, this message translates to:
  /// **'Confirm new password'**
  String get authResetConfirmPasswordLabel;

  /// Label of the reset-password submit button.
  ///
  /// In en, this message translates to:
  /// **'Reset password'**
  String get authResetSubmit;

  /// Confirmation shown after a successful password reset.
  ///
  /// In en, this message translates to:
  /// **'Password reset successfully. Please sign in with your new password.'**
  String get authResetSuccess;

  /// Shown when the reset screen is opened without a token.
  ///
  /// In en, this message translates to:
  /// **'This reset link is missing a token. Please open it from your email.'**
  String get authResetMissingToken;

  /// Tooltip for the show-password toggle.
  ///
  /// In en, this message translates to:
  /// **'Show password'**
  String get authShowPassword;

  /// Tooltip for the hide-password toggle.
  ///
  /// In en, this message translates to:
  /// **'Hide password'**
  String get authHidePassword;

  /// Tooltip and action label for signing out.
  ///
  /// In en, this message translates to:
  /// **'Log out'**
  String get authLogout;

  /// Error shown when login credentials are rejected.
  ///
  /// In en, this message translates to:
  /// **'Invalid email or password.'**
  String get authErrorsInvalidCredentials;

  /// Error shown when the account is temporarily locked.
  ///
  /// In en, this message translates to:
  /// **'Account locked due to too many failed attempts. Try again in 15 minutes.'**
  String get authErrorsAccountLocked;

  /// Error shown when a session cannot be refreshed.
  ///
  /// In en, this message translates to:
  /// **'Your session has expired. Please sign in again.'**
  String get authErrorsSessionExpired;

  /// Error shown for a rejected request (HTTP 400).
  ///
  /// In en, this message translates to:
  /// **'The request could not be processed. Please check your details.'**
  String get authErrorsBadRequest;

  /// Error shown when the device has no usable connection.
  ///
  /// In en, this message translates to:
  /// **'You seem to be offline. Check your connection and try again.'**
  String get authErrorsNetwork;

  /// Error shown when the backend rate-limits a request.
  ///
  /// In en, this message translates to:
  /// **'Too many attempts. Please try again later.'**
  String get authErrorsRateLimited;

  /// Error shown when the server fails (HTTP 5xx).
  ///
  /// In en, this message translates to:
  /// **'Something went wrong on our side. Please try again later.'**
  String get authErrorsServer;

  /// Fallback error message for unexpected failures.
  ///
  /// In en, this message translates to:
  /// **'Something went wrong. Please try again.'**
  String get authErrorsUnknown;

  /// Error shown when a required field is empty.
  ///
  /// In en, this message translates to:
  /// **'This field is required.'**
  String get validationRequired;

  /// Error shown when an email address is malformed.
  ///
  /// In en, this message translates to:
  /// **'Enter a valid email address.'**
  String get validationEmailInvalid;

  /// Error shown when a password is too short.
  ///
  /// In en, this message translates to:
  /// **'Password must be at least {min} characters.'**
  String validationPasswordTooShort(int min);

  /// Error shown when a password is too long.
  ///
  /// In en, this message translates to:
  /// **'Password must be at most {max} characters.'**
  String validationPasswordTooLong(int max);

  /// Error shown when a password lacks an uppercase letter.
  ///
  /// In en, this message translates to:
  /// **'Password must contain at least one uppercase letter.'**
  String get validationPasswordUppercase;

  /// Error shown when a password lacks a lowercase letter.
  ///
  /// In en, this message translates to:
  /// **'Password must contain at least one lowercase letter.'**
  String get validationPasswordLowercase;

  /// Error shown when a password lacks a digit.
  ///
  /// In en, this message translates to:
  /// **'Password must contain at least one number.'**
  String get validationPasswordDigit;

  /// Error shown when a password lacks a special character.
  ///
  /// In en, this message translates to:
  /// **'Password must contain at least one special character.'**
  String get validationPasswordSpecial;

  /// Error shown when the confirm-password field differs from the password.
  ///
  /// In en, this message translates to:
  /// **'Passwords do not match.'**
  String get validationPasswordMismatch;

  /// Error shown when a phone number is not a valid 10-digit Indian mobile number.
  ///
  /// In en, this message translates to:
  /// **'Enter a valid 10-digit mobile number.'**
  String get validationPhoneInvalid;
}

class _AppLocalizationsDelegate
    extends LocalizationsDelegate<AppLocalizations> {
  const _AppLocalizationsDelegate();

  @override
  Future<AppLocalizations> load(Locale locale) {
    return SynchronousFuture<AppLocalizations>(lookupAppLocalizations(locale));
  }

  @override
  bool isSupported(Locale locale) =>
      <String>['en', 'hi', 'mr'].contains(locale.languageCode);

  @override
  bool shouldReload(_AppLocalizationsDelegate old) => false;
}

AppLocalizations lookupAppLocalizations(Locale locale) {
  // Lookup logic when only language code is specified.
  switch (locale.languageCode) {
    case 'en':
      return AppLocalizationsEn();
    case 'hi':
      return AppLocalizationsHi();
    case 'mr':
      return AppLocalizationsMr();
  }

  throw FlutterError(
    'AppLocalizations.delegate failed to load unsupported locale "$locale". This is likely '
    'an issue with the localizations generation tool. Please file an issue '
    'on GitHub with a reproducible sample app and the gen-l10n configuration '
    'that was used.',
  );
}
