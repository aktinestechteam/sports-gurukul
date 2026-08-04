// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for Marathi (`mr`).
class AppLocalizationsMr extends AppLocalizations {
  AppLocalizationsMr([String locale = 'mr']) : super(locale);

  @override
  String get appTitle => 'स्पोर्ट्स गुरुकुल';

  @override
  String get appSplashTagline => 'प्रशिक्षण • स्पर्धा • उत्कृष्टता';

  @override
  String get appInitializing => 'स्पोर्ट्स गुरुकुल सुरू होत आहे…';

  @override
  String get dashboardInitializedTitle => 'प्रकल्प यशस्वीरित्या सुरू झाला';

  @override
  String get dashboardInitializedMessage =>
      'स्पोर्ट्स गुरुकुल मोबाइल पाया तयार आहे. पुढील स्प्रिंट्समध्ये वैशिष्ट्ये उपलब्ध होतील.';

  @override
  String dashboardGreetingMorning(String name) {
    return 'सुप्रभात, $name';
  }

  @override
  String dashboardGreetingAfternoon(String name) {
    return 'शुभ दुपार, $name';
  }

  @override
  String dashboardGreetingEvening(String name) {
    return 'शुभ संध्या, $name';
  }

  @override
  String get dashboardHeaderSubtitle => 'तुमची कामगिरी, एका दृष्टीक्षेपात.';

  @override
  String get dashboardStatActiveDays => 'सक्रिय दिवस';

  @override
  String get dashboardStatAvgIntensity => 'सरासरी तीव्रता';

  @override
  String get dashboardStatRecovery => 'पुनर्प्राप्ती';

  @override
  String get dashboardWeeklyTarget => 'साप्ताहिक लक्ष्य';

  @override
  String get dashboardWeeklyTargetDone => 'या आठवड्यात 24 पैकी 18 सत्र पूर्ण';

  @override
  String get dashboardQuickActions => 'जलद क्रिया';

  @override
  String get dashboardActionBookSession => 'सत्र बुक करा';

  @override
  String get dashboardActionFindCoach => 'कोच शोधा';

  @override
  String get dashboardActionLeaderboards => 'लीडरबोर्ड';

  @override
  String get dashboardActionTournaments => 'स्पर्धा';

  @override
  String get dashboardTabOverview => 'आढावा';

  @override
  String get dashboardTabTraining => 'प्रशिक्षण';

  @override
  String get dashboardTabInsights => 'अंतर्दृष्टी';

  @override
  String get dashboardUpcomingSessions => 'आगामी सत्रे';

  @override
  String get dashboardSessionEveningRun => 'संध्याकाळची धाव';

  @override
  String get dashboardSessionEveningRunTime => 'आज · सायं. 6:00';

  @override
  String get dashboardSessionSquadStrength => 'संघ स्ट्रेंथ';

  @override
  String get dashboardSessionSquadStrengthTime => 'बुध · सकाळी 5:30';

  @override
  String get dashboardInsightRecovery =>
      'या आठवड्यात पुनर्प्राप्ती स्कोर 12% वाढला';

  @override
  String get dashboardInsightIntensity => '4 दिवस लक्ष्य तीव्रता क्षेत्रात';

  @override
  String get dashboardInsightRestDay => 'बुधवारसाठी विश्रांतीचा दिवस नियोजित';

  @override
  String get authLoginTitle => 'स्वागत आहे!';

  @override
  String get authLoginSubtitle =>
      'स्पोर्ट्स गुरुकुल सुरू ठेवण्यासाठी साइन इन करा.';

  @override
  String get authLoginEmailLabel => 'ईमेल';

  @override
  String get authLoginPasswordLabel => 'पासवर्ड';

  @override
  String get authLoginSubmit => 'साइन इन करा';

  @override
  String get authLoginForgotPassword => 'पासवर्ड विसरलात?';

  @override
  String get authLoginRememberMe => 'मला लक्षात ठेवा';

  @override
  String get authLoginOr => 'किंवा';

  @override
  String get authLoginDontHaveAccount => 'तुमचे खाते नाही?';

  @override
  String get authLoginSignUp => 'साइन अप करा';

  @override
  String get authSignUpTitle => 'तुमचे खाते तयार करा';

  @override
  String get authSignUpSubtitle =>
      'तुमचा प्रवास सुरू करण्यासाठी स्पोर्ट्स गुरुकुल खाते तयार करा.';

  @override
  String get authSignUpNameLabel => 'पूर्ण नाव';

  @override
  String get authSignUpPhoneLabel => 'फोन नंबर';

  @override
  String get authSignUpConfirmPasswordLabel => 'पासवर्डची पुष्टी करा';

  @override
  String get authSignUpSubmit => 'खाते तयार करा';

  @override
  String get authSignUpAlreadyHaveAccount => 'आधीच खाते आहे?';

  @override
  String get authSignUpSignIn => 'साइन इन करा';

  @override
  String get authSignUpSuccess =>
      'खाते यशस्वीरित्या तयार झाले. तुम्ही साइन इन आहात.';

  @override
  String get authForgotTitle => 'पासवर्ड रीसेट करा';

  @override
  String get authForgotSubtitle =>
      'तुमचा ईमेल टाका, आम्ही तुम्हाला रीसेट लिंक पाठवू.';

  @override
  String get authForgotSubmit => 'रीसेट लिंक पाठवा';

  @override
  String get authForgotEmailSent =>
      'त्या ईमेलशी खाते अस्तित्वात असल्यास, रीसेट लिंक पाठवले आहे.';

  @override
  String get authForgotBackToLogin => 'साइन इनकडे परत जा';

  @override
  String get authResetTitle => 'नवीन पासवर्ड निवडा';

  @override
  String get authResetSubtitle =>
      'एक मजबूत पासवर्ड टाका जो पूर्वी वापरला नसेल.';

  @override
  String get authResetNewPasswordLabel => 'नवीन पासवर्ड';

  @override
  String get authResetConfirmPasswordLabel => 'नवीन पासवर्डची पुष्टी करा';

  @override
  String get authResetSubmit => 'पासवर्ड रीसेट करा';

  @override
  String get authResetSuccess =>
      'पासवर्ड यशस्वीरित्या रीसेट झाला. कृपया नवीन पासवर्डने साइन इन करा.';

  @override
  String get authResetMissingToken =>
      'या रीसेट लिंकमध्ये टोकन नाही. कृपया ती तुमच्या ईमेलमधून उघडा.';

  @override
  String get authShowPassword => 'पासवर्ड दाखवा';

  @override
  String get authHidePassword => 'पासवर्ड लपवा';

  @override
  String get authLogout => 'लॉग आउट';

  @override
  String get authErrorsInvalidCredentials => 'ईमेल किंवा पासवर्ड चुकीचा आहे.';

  @override
  String get authErrorsAccountLocked =>
      'अनेक अयशस्वी प्रयत्नांमुळे खाते लॉक झाले. 15 मिनिटांत पुन्हा प्रयत्न करा.';

  @override
  String get authErrorsSessionExpired =>
      'तुमचा सत्र संपला आहे. कृपया पुन्हा साइन इन करा.';

  @override
  String get authErrorsBadRequest =>
      'विनंती प्रक्रिया होऊ शकली नाही. कृपया तुमची माहिती तपासा.';

  @override
  String get authErrorsNetwork =>
      'तुम्ही ऑफलाइन आहात असे दिसते. कनेक्शन तपासून पुन्हा प्रयत्न करा.';

  @override
  String get authErrorsRateLimited =>
      'खूप जास्त प्रयत्न. कृपया नंतर पुन्हा प्रयत्न करा.';

  @override
  String get authErrorsServer =>
      'आमच्या बाजूने काहीतरी चूक झाली. कृपया नंतर पुन्हा प्रयत्न करा.';

  @override
  String get authErrorsUnknown => 'काहीतरी चूक झाली. कृपया पुन्हा प्रयत्न करा.';

  @override
  String get validationRequired => 'हे क्षेत्र आवश्यक आहे.';

  @override
  String get validationEmailInvalid => 'वैध ईमेल पत्ता टाका.';

  @override
  String validationPasswordTooShort(int min) {
    return 'पासवर्ड किमान $min अक्षरांचा असावा.';
  }

  @override
  String validationPasswordTooLong(int max) {
    return 'पासवर्ड जास्तीत जास्त $max अक्षरांचा असावा.';
  }

  @override
  String get validationPasswordUppercase =>
      'पासवर्डमध्ये किमान एक मोठे अक्षर असावे.';

  @override
  String get validationPasswordLowercase =>
      'पासवर्डमध्ये किमान एक लहान अक्षर असावे.';

  @override
  String get validationPasswordDigit => 'पासवर्डमध्ये किमान एक अंक असावा.';

  @override
  String get validationPasswordSpecial =>
      'पासवर्डमध्ये किमान एक विशेष चिन्ह असावे.';

  @override
  String get validationPasswordMismatch => 'पासवर्ड जुळत नाहीत.';

  @override
  String get validationPhoneInvalid => 'वैध 10-अंकीय मोबाइल नंबर टाका.';
}
