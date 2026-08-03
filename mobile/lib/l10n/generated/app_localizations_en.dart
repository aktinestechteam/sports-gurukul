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
}
