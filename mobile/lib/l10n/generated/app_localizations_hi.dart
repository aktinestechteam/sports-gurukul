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

  @override
  String get validationDateInvalid => 'एक मान्य तिथि दर्ज करें।';

  @override
  String get validationDateFuture => 'जन्म तिथि भविष्य में नहीं हो सकती।';

  @override
  String get validationNumberInvalid => 'एक मान्य संख्या दर्ज करें।';

  @override
  String get validationPostalCodeInvalid => 'एक मान्य पिन कोड दर्ज करें।';

  @override
  String get profileMyProfileTitle => 'मेरी प्रोफ़ाइल';

  @override
  String get profileRetry => 'पुनः प्रयास करें';

  @override
  String get profileErrorTitle => 'प्रोफ़ाइल लोड नहीं हो सकी';

  @override
  String get profileFullName => 'पूरा नाम';

  @override
  String get profileEmail => 'ईमेल';

  @override
  String get profilePhone => 'फ़ोन';

  @override
  String get profilePreferredSport => 'पसंदीदा खेल';

  @override
  String get profileExperienceLevel => 'अनुभव स्तर';

  @override
  String get profileHeight => 'ऊँचाई';

  @override
  String get profileWeight => 'वज़न';

  @override
  String get profileEditProfile => 'प्रोफ़ाइल संपादित करें';

  @override
  String get profileCreateProfile => 'प्रोफ़ाइल बनाएं';

  @override
  String get profileCreateTitle => 'प्रोफ़ाइल बनाएं';

  @override
  String get profileCreateSubtitle => 'अपने बारे में थोड़ी जानकारी दें।';

  @override
  String get profileCreatePrompt => 'आगे बढ़ने के लिए अपनी प्रोफ़ाइल बनाएं।';

  @override
  String get profileAboutMe => 'मेरे बारे में';

  @override
  String get profileAddresses => 'पते';

  @override
  String get profileEditTitle => 'प्रोफ़ाइल संपादित करें';

  @override
  String get profileEditSubtitle => 'अपनी जानकारी अद्यतन रखें।';

  @override
  String get profileEditBasicInfo => 'मूल जानकारी';

  @override
  String get profileGender => 'लिंग';

  @override
  String get profileGenderMale => 'पुरुष';

  @override
  String get profileGenderFemale => 'महिला';

  @override
  String get profileGenderNonBinary => 'नॉन-बाइनरी';

  @override
  String get profileGenderPreferNotToSay => 'कहना पसंद नहीं';

  @override
  String get profileBio => 'परिचय';

  @override
  String get profileEditContactInfo => 'संपर्क जानकारी';

  @override
  String get profileCountryCode => 'कोड';

  @override
  String get profileEditAddress => 'पता';

  @override
  String get profileAddressType => 'पते का प्रकार';

  @override
  String get profileAddressTypeHome => 'घर';

  @override
  String get profileAddressTypeWork => 'कार्यालय';

  @override
  String get profileAddressTypeAcademy => 'अकादमी';

  @override
  String get profileAddressTypeOther => 'अन्य';

  @override
  String get profileAddressLine1 => 'पता पंक्ति 1';

  @override
  String get profileAddressLine2 => 'पता पंक्ति 2';

  @override
  String get profileCity => 'शहर';

  @override
  String get profileState => 'राज्य';

  @override
  String get profileCountry => 'देश';

  @override
  String get profilePostalCode => 'पिन कोड';

  @override
  String get profileDateOfBirth => 'जन्म तिथि';

  @override
  String get profileSave => 'सहेजें';

  @override
  String get profileUpdateSuccess => 'प्रोफ़ाइल सफलतापूर्वक अपडेट हुई।';

  @override
  String get profilePreferencesTitle => 'प्राथमिकताएँ';

  @override
  String get profilePreferencesSubtitle =>
      'सूचनाएँ, दिखावट और गोपनीयता अनुकूलित करें।';

  @override
  String get profilePreferencesAppearance => 'दिखावट';

  @override
  String get profilePreferencesTheme => 'थीम';

  @override
  String get profileThemeLight => 'लाइट';

  @override
  String get profileThemeDark => 'डार्क';

  @override
  String get profileThemeSystem => 'सिस्टम';

  @override
  String get profilePreferencesNotifications => 'सूचनाएँ';

  @override
  String get profilePreferenceEmailNotifications => 'ईमेल सूचनाएँ';

  @override
  String get profilePreferencePushNotifications => 'पुश सूचनाएँ';

  @override
  String get profilePreferenceSmsNotifications => 'एसएमएस सूचनाएँ';

  @override
  String get profilePreferenceMarketingEmails => 'मार्केटिंग ईमेल';

  @override
  String get profilePreferencesPrivacy => 'गोपनीयता';

  @override
  String get profilePreferenceProfileVisibility => 'प्रोफ़ाइल दृश्यता';

  @override
  String get profilePreferenceShowOnlineStatus => 'ऑनलाइन स्थिति दिखाएँ';

  @override
  String get profilePreferencesSaved => 'प्राथमिकताएँ सफलतापूर्वक सहेजी गईं।';

  @override
  String get profileChangePhoto => 'फ़ोटो बदलें';

  @override
  String get profilePhotoUploaded => 'प्रोफ़ाइल फ़ोटो अपलोड हुई।';

  @override
  String get profilePhotoRemoved => 'प्रोफ़ाइल फ़ोटो हटाई गई।';

  @override
  String get profileRemovePhoto => 'फ़ोटो हटाएँ';

  @override
  String get profileErrorsNotFound => 'प्रोफ़ाइल नहीं मिली।';

  @override
  String get profileErrorsNetwork =>
      'आप ऑफ़लाइन लग रहे हैं। अपना कनेक्शन जाँचकर पुनः प्रयास करें।';

  @override
  String get profileErrorsServer =>
      'हमारी ओर से कोई समस्या आई। कृपया बाद में पुनः प्रयास करें।';

  @override
  String get profileErrorsValidation =>
      'अनुरोध संसाधित नहीं हो सका। कृपया अपनी जानकारी जाँचें।';

  @override
  String get profileErrorsPhotoUpload =>
      'फ़ोटो अपलोड नहीं हो सकी। कृपया 5 MB से कम की JPEG, PNG या WebP छवि प्रयास करें।';

  @override
  String get profileErrorsPhotoNotFound => 'कोई प्रोफ़ाइल फ़ोटो नहीं मिली।';

  @override
  String get profileErrorsPermission =>
      'आपके पास यह क्रिया करने की अनुमति नहीं है।';

  @override
  String get profileErrorsUnknown => 'कुछ गलत हो गया। कृपया पुनः प्रयास करें।';

  @override
  String get welcomeTitle => 'स्पोर्ट्स गुरुकुल में आपका स्वागत है!';

  @override
  String get welcomeSubtitle =>
      'आप तैयार हैं। अपनी खेल यात्रा शुरू करने के लिए एक रास्ता चुनें।';

  @override
  String get welcomeCreateAcademy => 'मेरी अकादमी बनाएँ';

  @override
  String get welcomeCreateAcademySubtitle =>
      'नई अकादमी स्थापित करें और एथलीटों का प्रबंधन शुरू करें।';

  @override
  String get welcomeJoinAcademy => 'मौजूदा अकादमी से जुड़ें';

  @override
  String get welcomeJoinAcademySubtitle =>
      'एक अकादमी खोजें और सदस्य के रूप में जुड़ें।';

  @override
  String get welcomeExplore => 'एप्लिकेशन देखें';

  @override
  String get welcomeExploreSubtitle => 'सेटअप छोड़ें और अभी डैशबोर्ड देखें।';

  @override
  String get welcomeLoading => 'आपका अनुभव तैयार हो रहा है…';

  @override
  String get welcomeEmptyMessage => 'अभी कोई सत्र नहीं है।';

  @override
  String get welcomeErrorTitle => 'आपका खाता लोड नहीं हो सका';

  @override
  String get welcomeErrorsNetwork =>
      'आप ऑफ़लाइन लग रहे हैं। अपना कनेक्शन जाँचकर पुनः प्रयास करें।';

  @override
  String get welcomeErrorsServer =>
      'हमारी ओर से कोई समस्या आई। कृपया बाद में पुनः प्रयास करें।';

  @override
  String get welcomeErrorsSessionExpired =>
      'आपका सत्र समाप्त हो गया। कृपया फिर से साइन इन करें।';

  @override
  String get welcomeErrorsUnknown => 'कुछ गलत हो गया। कृपया पुनः प्रयास करें।';

  @override
  String get welcomeRetry => 'पुनः प्रयास करें';

  @override
  String get academyCreateTitle => 'अकादमी बनाएँ';

  @override
  String get academyCreateMessage =>
      'अकादमी सेटअप आगामी स्प्रिंट में आ रहा है। इस बीच आप एप्लिकेशन देख सकते हैं।';

  @override
  String get academyJoinTitle => 'किसी अकादमी से जुड़ें';

  @override
  String get academyJoinMessage =>
      'अकादमी खोजना और जुड़ना आगामी स्प्रिंट में आ रहा है। इस बीच आप एप्लिकेशन देख सकते हैं।';

  @override
  String get academyBackToDashboard => 'डैशबोर्ड पर वापस जाएँ';

  @override
  String get academyCreateSubtitle =>
      'हमें अपनी अकादमी के बारे में बताएं और कुछ चरणों में इसे सेट करें।';

  @override
  String academyStepIndicator(int current, int total) {
    return 'चरण $current / $total';
  }

  @override
  String get academyStepBasics => 'मूल जानकारी';

  @override
  String get academyStepContact => 'संपर्क जानकारी';

  @override
  String get academyStepAddress => 'पता';

  @override
  String get academyStepBranding => 'ब्रांडिंग';

  @override
  String get academyStepReview => 'समीक्षा करें और सबमिट करें';

  @override
  String get academyNameLabel => 'अकादमी का नाम';

  @override
  String get academyDescriptionLabel => 'विवरण';

  @override
  String get academyTypeLabel => 'अकादमी का प्रकार';

  @override
  String get academyTypeSingleSport => 'एकल-खेल';

  @override
  String get academyTypeSingleSportHint => 'एक खेल पर ध्यान केंद्रित करें।';

  @override
  String get academyTypeMultiSport => 'बहु-खेल';

  @override
  String get academyTypeMultiSportHint => 'कई खेलों की पेशकश करें।';

  @override
  String get academySportsLabel => 'उपलब्ध खेल';

  @override
  String get academySelectAtLeastOneSport => 'कम से कम एक खेल चुनें।';

  @override
  String get academyContactPersonLabel => 'संपर्क व्यक्ति';

  @override
  String get academyEmailLabel => 'अकादमी ईमेल';

  @override
  String get academyPhoneLabel => 'मोबाइल नंबर';

  @override
  String get academyWebsiteLabel => 'वेबसाइट (वैकल्पिक)';

  @override
  String get academyCountryLabel => 'देश';

  @override
  String get academyStateLabel => 'राज्य';

  @override
  String get academyCityLabel => 'शहर';

  @override
  String get academyAddressLineLabel => 'पता पंक्ति';

  @override
  String get academyPostalCodeLabel => 'पोस्टल कोड (वैकल्पिक)';

  @override
  String get academyLogoLabel => 'अकादमी लोगो';

  @override
  String get academyLogoRequired => 'कृपया अकादमी लोगो जोड़ें।';

  @override
  String get academyLogoHint =>
      'एक स्पष्ट वर्गाकार लोगो (JPEG, PNG या WebP, 5 MB से कम)।';

  @override
  String get academyCoverLabel => 'कवर छवि';

  @override
  String get academyCoverHint => 'वैकल्पिक। चौड़ी बैनर छवि सबसे अच्छी लगती है।';

  @override
  String get academyChooseImage => 'गैलरी से चुनें';

  @override
  String get academyReplaceImage => 'बदलें';

  @override
  String get academyRemoveImage => 'हटाएं';

  @override
  String get academyReviewTitle => 'समीक्षा करें और सबमिट करें';

  @override
  String get academyReviewSubtitle =>
      'अपनी अकादमी बनाने से पहले सब कुछ सही है, जांच लें।';

  @override
  String get academyReviewEdit => 'संपादित करें';

  @override
  String get academyReviewNotProvided => 'उपलब्ध नहीं है';

  @override
  String get academyBackButton => 'वापस';

  @override
  String get academyNextButton => 'जारी रखें';

  @override
  String get academySubmitButton => 'अकादमी बनाएं';

  @override
  String get academyEditTitle => 'अकादमी संपादित करें';

  @override
  String get academyEditSubtitle =>
      'अपनी अकादमी का विवरण अपडेट करें और बदलाव सहेजें।';

  @override
  String get academyEditSaveButton => 'बदलाव सहेजें';

  @override
  String get academyTypeSportsLocked =>
      'बनाने के बाद अकादमी का प्रकार और खेल नहीं बदले जा सकते।';

  @override
  String academySubmitSuccess(String name) {
    return '$name बन गया है। स्पोर्ट्स गुरुकुल में आपका स्वागत है!';
  }

  @override
  String get academyErrorsNetwork =>
      'आप ऑफ़लाइन लगते हैं। अपना कनेक्शन जांचें और फिर से कोशिश करें।';

  @override
  String get academyErrorsServer =>
      'हमारी ओर से कुछ गड़बड़ हुई। कृपया बाद में फिर से प्रयास करें।';

  @override
  String get academyErrorsValidation =>
      'अनुरोध संसाधित नहीं किया जा सका। कृपया अपनी जानकारी जांचें।';

  @override
  String get academyErrorsPermission =>
      'आपके पास अकादमी बनाने की अनुमति नहीं है।';

  @override
  String get academyErrorsUnknown =>
      'कुछ गड़बड़ हुई। कृपया फिर से प्रयास करें।';

  @override
  String get validationUrlInvalid => 'मान्य URL दर्ज करें।';

  @override
  String get roleLabelPlatformAdministrator => 'प्लेटफ़ॉर्म प्रशासक';

  @override
  String get roleLabelAcademy => 'अकादमी';

  @override
  String get roleLabelCoach => 'कोच';

  @override
  String get roleLabelAthlete => 'एथलीट';

  @override
  String get roleLabelParent => 'अभिभावक';

  @override
  String get roleLabelScout => 'स्काउट';

  @override
  String get roleLabelSponsor => 'प्रायोजक';

  @override
  String get roleLabelAiAdministrator => 'एआई प्रशासक';

  @override
  String get roleLabelMember => 'सदस्य';

  @override
  String get roleLabelNewUser => 'नया उपयोगकर्ता';

  @override
  String get roleLabelRegisteredUser => 'पंजीकृत उपयोगकर्ता';

  @override
  String get roleLabelAcademyAdmin => 'अकादमी प्रशासक';

  @override
  String get roleLabelPendingApproval => 'अनुमोदन लंबित';

  @override
  String get roleLabelSystemAdmin => 'सिस्टम प्रशासक';
}
