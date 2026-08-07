import 'package:sports_gurukul/app/router/route_paths.dart' show RoutePaths;

/// Centralized route names for the Sports Gurukul application.
///
/// Route names are used as identifiers and must stay in sync with
/// [RoutePaths]. Never reference raw route strings inside features.
abstract final class RouteNames {
  static const String splash = 'splash';
  static const String login = 'login';
  static const String signUp = 'signUp';
  static const String forgotPassword = 'forgotPassword';
  static const String resetPassword = 'resetPassword';
  static const String welcome = 'welcome';
  static const String createAcademy = 'createAcademy';
  static const String editAcademy = 'editAcademy';
  static const String joinAcademy = 'joinAcademy';
  static const String dashboard = 'dashboard';
  static const String academyDashboard = 'academyDashboard';
  static const String profile = 'profile';
  static const String editProfile = 'editProfile';
  static const String editPreferences = 'editPreferences';
}
