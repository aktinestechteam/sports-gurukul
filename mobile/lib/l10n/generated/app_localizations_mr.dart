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

  @override
  String get validationDateInvalid => 'वैध तारीख प्रविष्ट करा.';

  @override
  String get validationDateFuture => 'जन्मतारीख भविष्यात असू शकत नाही.';

  @override
  String get validationNumberInvalid => 'वैध संख्या प्रविष्ट करा.';

  @override
  String get validationPostalCodeInvalid => 'वैध पिन कोड प्रविष्ट करा.';

  @override
  String get profileMyProfileTitle => 'माझी प्रोफाइल';

  @override
  String get profileRetry => 'पुन्हा प्रयत्न करा';

  @override
  String get profileErrorTitle => 'प्रोफाइल लोड होऊ शकली नाही';

  @override
  String get profileFullName => 'पूर्ण नाव';

  @override
  String get profileEmail => 'ईमेल';

  @override
  String get profilePhone => 'फोन';

  @override
  String get profilePreferredSport => 'आवडता खेळ';

  @override
  String get profileExperienceLevel => 'अनुभव पातळी';

  @override
  String get profileHeight => 'उंची';

  @override
  String get profileWeight => 'वजन';

  @override
  String get profileEditProfile => 'प्रोफाइल संपादित करा';

  @override
  String get profileCreateProfile => 'प्रोफाइल तयार करा';

  @override
  String get profileCreateTitle => 'प्रोफाइल तयार करा';

  @override
  String get profileCreateSubtitle => 'तुमच्याबद्दल थोडी माहिती द्या.';

  @override
  String get profileCreatePrompt => 'पुढे जाण्यासाठी तुमचे प्रोफाइल तयार करा.';

  @override
  String get profileAboutMe => 'माझ्याबद्दल';

  @override
  String get profileAddresses => 'पत्ते';

  @override
  String get profileEditTitle => 'प्रोफाइल संपादित करा';

  @override
  String get profileEditSubtitle => 'तुमची माहिती अद्ययावत ठेवा.';

  @override
  String get profileEditBasicInfo => 'मूलभूत माहिती';

  @override
  String get profileGender => 'लिंग';

  @override
  String get profileGenderMale => 'पुरुष';

  @override
  String get profileGenderFemale => 'स्त्री';

  @override
  String get profileGenderNonBinary => 'नॉन-बायनरी';

  @override
  String get profileGenderPreferNotToSay => 'सांगण्यास प्राधान्य नाही';

  @override
  String get profileBio => 'ओळख';

  @override
  String get profileEditContactInfo => 'संपर्क माहिती';

  @override
  String get profileCountryCode => 'कोड';

  @override
  String get profileEditAddress => 'पत्ता';

  @override
  String get profileAddressType => 'पत्त्याचा प्रकार';

  @override
  String get profileAddressTypeHome => 'घर';

  @override
  String get profileAddressTypeWork => 'कार्यालय';

  @override
  String get profileAddressTypeAcademy => 'अकादमी';

  @override
  String get profileAddressTypeOther => 'इतर';

  @override
  String get profileAddressLine1 => 'पत्ता ओळ 1';

  @override
  String get profileAddressLine2 => 'पत्ता ओळ 2';

  @override
  String get profileCity => 'शहर';

  @override
  String get profileState => 'राज्य';

  @override
  String get profileCountry => 'देश';

  @override
  String get profilePostalCode => 'पिन कोड';

  @override
  String get profileDateOfBirth => 'जन्म तारीख';

  @override
  String get profileSave => 'सेव्ह करा';

  @override
  String get profileUpdateSuccess => 'प्रोफाइल यशस्वीरित्या अद्ययावत केली.';

  @override
  String get profilePreferencesTitle => 'प्राधान्ये';

  @override
  String get profilePreferencesSubtitle =>
      'सूचना, स्वरूप आणि गोपनीयता सानुकूलित करा.';

  @override
  String get profilePreferencesAppearance => 'स्वरूप';

  @override
  String get profilePreferencesTheme => 'थीम';

  @override
  String get profileThemeLight => 'लाइट';

  @override
  String get profileThemeDark => 'डार्क';

  @override
  String get profileThemeSystem => 'सिस्टम';

  @override
  String get profilePreferencesNotifications => 'सूचना';

  @override
  String get profilePreferenceEmailNotifications => 'ईमेल सूचना';

  @override
  String get profilePreferencePushNotifications => 'पुश सूचना';

  @override
  String get profilePreferenceSmsNotifications => 'एसएमएस सूचना';

  @override
  String get profilePreferenceMarketingEmails => 'मार्केटिंग ईमेल';

  @override
  String get profilePreferencesPrivacy => 'गोपनीयता';

  @override
  String get profilePreferenceProfileVisibility => 'प्रोफाइल दृश्यता';

  @override
  String get profilePreferenceShowOnlineStatus => 'ऑनलाइन स्थिती दाखवा';

  @override
  String get profilePreferencesSaved => 'प्राधान्ये यशस्वीरित्या सेव्ह केली.';

  @override
  String get profileChangePhoto => 'फोटो बदला';

  @override
  String get profilePhotoUploaded => 'प्रोफाइल फोटो अपलोड झाला.';

  @override
  String get profilePhotoRemoved => 'प्रोफाइल फोटो काढला गेला.';

  @override
  String get profileRemovePhoto => 'फोटो काढा';

  @override
  String get profileErrorsNotFound => 'प्रोफाइल सापडली नाही.';

  @override
  String get profileErrorsNetwork =>
      'तुम्ही ऑफलाइन आहात असे दिसते. कनेक्शन तपासून पुन्हा प्रयत्न करा.';

  @override
  String get profileErrorsServer =>
      'आमच्या बाजूने काहीतरी चूक झाली. कृपया नंतर पुन्हा प्रयत्न करा.';

  @override
  String get profileErrorsValidation =>
      'विनंती प्रक्रिया होऊ शकली नाही. कृपया तुमची माहिती तपासा.';

  @override
  String get profileErrorsPhotoUpload =>
      'फोटो अपलोड होऊ शकला नाही. कृपया 5 MB पेक्षा कमी JPEG, PNG किंवा WebP प्रतिमा वापरा.';

  @override
  String get profileErrorsPhotoNotFound => 'प्रोफाइल फोटो सापडला नाही.';

  @override
  String get profileErrorsPermission =>
      'ही क्रिया करण्याची परवानगी तुमच्याकडे नाही.';

  @override
  String get profileErrorsUnknown =>
      'काहीतरी चूक झाली. कृपया पुन्हा प्रयत्न करा.';

  @override
  String get welcomeTitle => 'स्पोर्ट्स गुरुकुलमध्ये आपले स्वागत आहे!';

  @override
  String get welcomeSubtitle =>
      'तुम्ही तयार आहात. तुमचा क्रीडा प्रवास सुरू करण्यासाठी एक मार्ग निवडा.';

  @override
  String get welcomeCreateAcademy => 'माझी अकादमी तयार करा';

  @override
  String get welcomeCreateAcademySubtitle =>
      'नवीन अकादमी स्थापन करा आणि खेळाडूंचे व्यवस्थापन सुरू करा.';

  @override
  String get welcomeJoinAcademy => 'विद्यमान अकादमीत सामील व्हा';

  @override
  String get welcomeJoinAcademySubtitle =>
      'अकादमी शोधा आणि सदस्य म्हणून सामील व्हा.';

  @override
  String get welcomeExplore => 'अॅप एक्सप्लोर करा';

  @override
  String get welcomeExploreSubtitle => 'सेटअप वगळा आणि आत्ताच डॅशबोर्ड पहा.';

  @override
  String get welcomeLoading => 'तुमचा अनुभव तयार होत आहे…';

  @override
  String get welcomeEmptyMessage => 'सध्या सत्र नाही.';

  @override
  String get welcomeErrorTitle => 'तुमचे खाते लोड होऊ शकले नाही';

  @override
  String get welcomeErrorsNetwork =>
      'तुम्ही ऑफलाइन आहात असे दिसते. कनेक्शन तपासून पुन्हा प्रयत्न करा.';

  @override
  String get welcomeErrorsServer =>
      'आमच्या बाजूने काहीतरी चूक झाली. कृपया नंतर पुन्हा प्रयत्न करा.';

  @override
  String get welcomeErrorsSessionExpired =>
      'तुमचे सत्र संपले आहे. कृपया पुन्हा साइन इन करा.';

  @override
  String get welcomeErrorsUnknown =>
      'काहीतरी चूक झाली. कृपया पुन्हा प्रयत्न करा.';

  @override
  String get welcomeRetry => 'पुन्हा प्रयत्न करा';

  @override
  String get academyCreateTitle => 'अकादमी तयार करा';

  @override
  String get academyCreateMessage =>
      'अकादमी सेटअप पुढील स्प्रिंटमध्ये येत आहे. तोपर्यंत तुम्ही अॅप एक्सप्लोर करू शकता.';

  @override
  String get academyJoinTitle => 'अकादमीत सामील व्हा';

  @override
  String get academyJoinMessage =>
      'अकादमी शोधणे आणि सामील होणे पुढील स्प्रिंटमध्ये येत आहे. तोपर्यंत तुम्ही अॅप एक्सप्लोर करू शकता.';

  @override
  String get academyBackToDashboard => 'डॅशबोर्डवर परत जा';

  @override
  String get academyCreateSubtitle =>
      'तुमच्या अकादमीबद्दल सांगा आणि काही चरणांमध्ये ती सेट करा.';

  @override
  String academyStepIndicator(int current, int total) {
    return 'चरण $current / $total';
  }

  @override
  String get academyStepBasics => 'मूल माहिती';

  @override
  String get academyStepContact => 'संपर्क माहिती';

  @override
  String get academyStepAddress => 'पत्ता';

  @override
  String get academyStepBranding => 'ब्रँडिंग';

  @override
  String get academyStepReview => 'पुनरावलोकन करा आणि सबमिट करा';

  @override
  String get academyNameLabel => 'अकादमीचे नाव';

  @override
  String get academyDescriptionLabel => 'वर्णन';

  @override
  String get academyTypeLabel => 'अकादमीचा प्रकार';

  @override
  String get academyTypeSingleSport => 'एक-खेळ';

  @override
  String get academyTypeSingleSportHint => 'एका खेळावर लक्ष केंद्रित करा.';

  @override
  String get academyTypeMultiSport => 'बहु-खेळ';

  @override
  String get academyTypeMultiSportHint => 'अनेक खेळ द्या.';

  @override
  String get academySportsLabel => 'उपलब्ध खेळ';

  @override
  String get academySelectAtLeastOneSport => 'किमान एक खेळ निवडा.';

  @override
  String get academyContactPersonLabel => 'संपर्क व्यक्ती';

  @override
  String get academyEmailLabel => 'अकादमी ईमेल';

  @override
  String get academyPhoneLabel => 'मोबाईल क्रमांक';

  @override
  String get academyWebsiteLabel => 'संकेतस्थळ (ऐच्छिक)';

  @override
  String get academyCountryLabel => 'देश';

  @override
  String get academyStateLabel => 'राज्य';

  @override
  String get academyCityLabel => 'शहर';

  @override
  String get academyAddressLineLabel => 'पत्त्याची ओळ';

  @override
  String get academyPostalCodeLabel => 'पिन कोड (ऐच्छिक)';

  @override
  String get academyLogoLabel => 'अकादमी लोगो';

  @override
  String get academyLogoRequired => 'कृपया अकादमी लोगो जोडा.';

  @override
  String get academyLogoHint =>
      'स्पष्ट चौकोनी लोगो (JPEG, PNG किंवा WebP, 5 MB पेक्षा कमी).';

  @override
  String get academyCoverLabel => 'कव्हर प्रतिमा';

  @override
  String get academyCoverHint => 'ऐच्छिक. रुंद बॅनर प्रतिमा उत्तम दिसते.';

  @override
  String get academyChooseImage => 'गॅलरीतून निवडा';

  @override
  String get academyReplaceImage => 'बदला';

  @override
  String get academyRemoveImage => 'काढा';

  @override
  String get academyReviewTitle => 'पुनरावलोकन करा आणि सबमिट करा';

  @override
  String get academyReviewSubtitle =>
      'अकादमी तयार करण्यापूर्वी सर्व काही व्यवस्थित आहे का ते तपासा.';

  @override
  String get academyReviewEdit => 'संपादित करा';

  @override
  String get academyReviewNotProvided => 'उपलब्ध नाही';

  @override
  String get academyBackButton => 'मागे';

  @override
  String get academyNextButton => 'पुढे जा';

  @override
  String get academySubmitButton => 'अकादमी तयार करा';

  @override
  String get academyEditTitle => 'अकादमी संपादित करा';

  @override
  String get academyEditSubtitle =>
      'तुमच्या अकादमीची माहिती अपडेट करा आणि बदल जतन करा.';

  @override
  String get academyEditSaveButton => 'बदल जतन करा';

  @override
  String get academyTypeSportsLocked =>
      'तयार केल्यानंतर अकादमीचा प्रकार आणि खेळ बदलता येत नाहीत.';

  @override
  String academySubmitSuccess(String name) {
    return '$name तयार झाली आहे. स्पोर्ट्स गुरुकुलमध्ये आपले स्वागत आहे!';
  }

  @override
  String get academyErrorsNetwork =>
      'तुम्ही ऑफलाइन दिसता. तुमचे कनेक्शन तपासा आणि पुन्हा प्रयत्न करा.';

  @override
  String get academyErrorsServer =>
      'आमच्या बाजूने काहीतरी चूक झाली. कृपया नंतर पुन्हा प्रयत्न करा.';

  @override
  String get academyErrorsValidation =>
      'विनंती प्रक्रिया करता आली नाही. कृपया तुमची माहिती तपासा.';

  @override
  String get academyErrorsPermission =>
      'अकादमी तयार करण्याची परवानगी तुमच्याकडे नाही.';

  @override
  String get academyErrorsUnknown =>
      'काहीतरी चूक झाली. कृपया पुन्हा प्रयत्न करा.';

  @override
  String get validationUrlInvalid => 'वैध URL प्रविष्ट करा.';

  @override
  String get roleLabelPlatformAdministrator => 'प्लॅटफॉर्म प्रशासक';

  @override
  String get roleLabelAcademy => 'अकादमी';

  @override
  String get roleLabelCoach => 'कोच';

  @override
  String get roleLabelAthlete => 'खेळाडू';

  @override
  String get roleLabelParent => 'पालक';

  @override
  String get roleLabelScout => 'स्काउट';

  @override
  String get roleLabelSponsor => 'प्रायोजक';

  @override
  String get roleLabelAiAdministrator => 'एआय प्रशासक';

  @override
  String get roleLabelMember => 'सदस्य';

  @override
  String get roleLabelNewUser => 'नवीन सदस्य';

  @override
  String get roleLabelRegisteredUser => 'नोंदणीकृत सदस्य';

  @override
  String get roleLabelAcademyAdmin => 'अकादमी प्रशासक';

  @override
  String get roleLabelPendingApproval => 'मंजुरी प्रलंबित';

  @override
  String get roleLabelSystemAdmin => 'सिस्टम प्रशासक';
}
