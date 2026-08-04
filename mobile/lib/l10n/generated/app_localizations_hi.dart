// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for Hindi (`hi`).
class AppLocalizationsHi extends AppLocalizations {
  AppLocalizationsHi([String locale = 'hi']) : super(locale);

  @override
  String get appTitle => 'स्पोर्ट्स गुरुकुल';

  @override
  String get appSplashTagline => 'प्रशिक्षण • प्रतिस्पर्धा • उत्कृष्टता';

  @override
  String get appInitializing => 'स्पोर्ट्स गुरुकुल आरंभ हो रहा है…';

  @override
  String get dashboardInitializedTitle => 'प्रोजेक्ट सफलतापूर्वक आरंभ हुआ';

  @override
  String get dashboardInitializedMessage =>
      'स्पोर्ट्स गुरुकुल मोबाइल आधार तैयार है। आगामी स्प्रिंट्स में सुविधाएँ वितरित की जाएँगी।';

  @override
  String dashboardGreetingMorning(String name) {
    return 'सुप्रभात, $name';
  }

  @override
  String dashboardGreetingAfternoon(String name) {
    return 'शुभ अपराह्न, $name';
  }

  @override
  String dashboardGreetingEvening(String name) {
    return 'शुभ संध्या, $name';
  }

  @override
  String get dashboardHeaderSubtitle => 'आपका प्रदर्शन, एक नज़र में।';

  @override
  String get dashboardStatActiveDays => 'सक्रिय दिन';

  @override
  String get dashboardStatAvgIntensity => 'औसत तीव्रता';

  @override
  String get dashboardStatRecovery => 'पुनर्प्राप्ति';

  @override
  String get dashboardWeeklyTarget => 'साप्ताहिक लक्ष्य';

  @override
  String get dashboardWeeklyTargetDone => 'इस सप्ताह 24 में से 18 सत्र पूर्ण';

  @override
  String get dashboardQuickActions => 'त्वरित क्रियाएँ';

  @override
  String get dashboardActionBookSession => 'सत्र बुक करें';

  @override
  String get dashboardActionFindCoach => 'कोच खोजें';

  @override
  String get dashboardActionLeaderboards => 'लीडरबोर्ड';

  @override
  String get dashboardActionTournaments => 'टूर्नामेंट';

  @override
  String get dashboardTabOverview => 'अवलोकन';

  @override
  String get dashboardTabTraining => 'प्रशिक्षण';

  @override
  String get dashboardTabInsights => 'अंतर्दृष्टि';

  @override
  String get dashboardUpcomingSessions => 'आगामी सत्र';

  @override
  String get dashboardSessionEveningRun => 'शाम की दौड़';

  @override
  String get dashboardSessionEveningRunTime => 'आज · शाम 6:00 बजे';

  @override
  String get dashboardSessionSquadStrength => 'टीम स्ट्रेंथ';

  @override
  String get dashboardSessionSquadStrengthTime => 'बुध · सुबह 5:30 बजे';

  @override
  String get dashboardInsightRecovery =>
      'पुनर्प्राप्ति स्कोर इस सप्ताह 12% बढ़ा';

  @override
  String get dashboardInsightIntensity => '4 दिन लक्ष्य तीव्रता क्षेत्र में';

  @override
  String get dashboardInsightRestDay => 'बुधवार के लिए विश्राम दिवस अनुकूलित';

  @override
  String get authLoginTitle => 'स्वागत है!';

  @override
  String get authLoginSubtitle =>
      'स्पोर्ट्स गुरुकुल जारी रखने के लिए साइन इन करें।';

  @override
  String get authLoginEmailLabel => 'ईमेल';

  @override
  String get authLoginPasswordLabel => 'पासवर्ड';

  @override
  String get authLoginSubmit => 'साइन इन करें';

  @override
  String get authLoginForgotPassword => 'पासवर्ड भूल गए?';

  @override
  String get authLoginRememberMe => 'मुझे याद रखें';

  @override
  String get authLoginOr => 'या';

  @override
  String get authLoginDontHaveAccount => 'क्या आपके पास खाता नहीं है?';

  @override
  String get authLoginSignUp => 'साइन अप करें';

  @override
  String get authSignUpTitle => 'अपना खाता बनाएं';

  @override
  String get authSignUpSubtitle =>
      'अपनी यात्रा शुरू करने के लिए स्पोर्ट्स गुरुकुल खाता बनाएं।';

  @override
  String get authSignUpNameLabel => 'पूरा नाम';

  @override
  String get authSignUpPhoneLabel => 'फ़ोन नंबर';

  @override
  String get authSignUpConfirmPasswordLabel => 'पासवर्ड की पुष्टि करें';

  @override
  String get authSignUpSubmit => 'खाता बनाएं';

  @override
  String get authSignUpAlreadyHaveAccount => 'क्या आपके पास पहले से खाता है?';

  @override
  String get authSignUpSignIn => 'साइन इन करें';

  @override
  String get authSignUpSuccess => 'खाता सफलतापूर्वक बन गया। आप साइन इन हैं।';

  @override
  String get authForgotTitle => 'पासवर्ड रीसेट करें';

  @override
  String get authForgotSubtitle =>
      'अपना ईमेल दर्ज करें, हम आपको रीसेट लिंक भेजेंगे।';

  @override
  String get authForgotSubmit => 'रीसेट लिंक भेजें';

  @override
  String get authForgotEmailSent =>
      'यदि उस ईमेल से कोई खाता मौजूद है, तो रीसेट लिंक भेज दिया गया है।';

  @override
  String get authForgotBackToLogin => 'साइन इन पर वापस जाएँ';

  @override
  String get authResetTitle => 'नया पासवर्ड चुनें';

  @override
  String get authResetSubtitle =>
      'एक मजबूत पासवर्ड दर्ज करें जो पहले उपयोग न हुआ हो।';

  @override
  String get authResetNewPasswordLabel => 'नया पासवर्ड';

  @override
  String get authResetConfirmPasswordLabel => 'नए पासवर्ड की पुष्टि करें';

  @override
  String get authResetSubmit => 'पासवर्ड रीसेट करें';

  @override
  String get authResetSuccess =>
      'पासवर्ड सफलतापूर्वक रीसेट हुआ। कृपया अपने नए पासवर्ड से साइन इन करें।';

  @override
  String get authResetMissingToken =>
      'इस रीसेट लिंक में टोकन नहीं है। कृपया इसे अपने ईमेल से खोलें।';

  @override
  String get authShowPassword => 'पासवर्ड दिखाएँ';

  @override
  String get authHidePassword => 'पासवर्ड छिपाएँ';

  @override
  String get authLogout => 'लॉग आउट';

  @override
  String get authErrorsInvalidCredentials => 'ईमेल या पासवर्ड गलत है।';

  @override
  String get authErrorsAccountLocked =>
      'कई असफल प्रयासों के कारण खाता लॉक हो गया। 15 मिनट में पुनः प्रयास करें।';

  @override
  String get authErrorsSessionExpired =>
      'आपका सत्र समाप्त हो गया है। कृपया पुनः साइन इन करें।';

  @override
  String get authErrorsBadRequest =>
      'अनुरोध संसाधित नहीं हो सका। कृपया अपनी जानकारी जाँचें।';

  @override
  String get authErrorsNetwork =>
      'आप ऑफ़लाइन लग रहे हैं। अपना कनेक्शन जाँचकर पुनः प्रयास करें।';

  @override
  String get authErrorsRateLimited =>
      'बहुत अधिक प्रयास। कृपया बाद में पुनः प्रयास करें।';

  @override
  String get authErrorsServer =>
      'हमारी ओर से कोई समस्या आई। कृपया बाद में पुनः प्रयास करें।';

  @override
  String get authErrorsUnknown => 'कुछ गलत हो गया। कृपया पुनः प्रयास करें।';

  @override
  String get validationRequired => 'यह फ़ील्ड आवश्यक है।';

  @override
  String get validationEmailInvalid => 'एक मान्य ईमेल पता दर्ज करें।';

  @override
  String validationPasswordTooShort(int min) {
    return 'पासवर्ड कम से कम $min अक्षरों का होना चाहिए।';
  }

  @override
  String validationPasswordTooLong(int max) {
    return 'पासवर्ड अधिकतम $max अक्षरों का होना चाहिए।';
  }

  @override
  String get validationPasswordUppercase =>
      'पासवर्ड में कम से कम एक बड़ा अक्षर होना चाहिए।';

  @override
  String get validationPasswordLowercase =>
      'पासवर्ड में कम से कम एक छोटा अक्षर होना चाहिए।';

  @override
  String get validationPasswordDigit =>
      'पासवर्ड में कम से कम एक संख्या होनी चाहिए।';

  @override
  String get validationPasswordSpecial =>
      'पासवर्ड में कम से कम एक विशेष वर्ण होना चाहिए।';

  @override
  String get validationPasswordMismatch => 'पासवर्ड मेल नहीं खाते।';

  @override
  String get validationPhoneInvalid =>
      'एक मान्य 10-अंकीय मोबाइल नंबर दर्ज करें।';
}
