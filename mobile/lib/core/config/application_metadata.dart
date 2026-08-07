import 'package:sports_gurukul/core/constants/app_constants.dart';

/// Static, build-time metadata about the application.
///
/// Populated during startup from `package_info_plus` (see
/// `core/info/app_info.dart`) and attached to a `BuildEnvironment` for use
/// in diagnostics, version gates and support screens.
class ApplicationMetadata {
  const ApplicationMetadata({
    this.appName = AppConstants.appName,
    this.packageName = '',
    this.version = '',
    this.buildNumber = '',
  });

  /// The display name of the application.
  final String appName;

  /// Reverse-DNS package/bundle identifier.
  final String packageName;

  /// Semantic version, e.g. `1.0.0`.
  final String version;

  /// Build number as reported by the platform.
  final String buildNumber;

  @override
  String toString() =>
      'ApplicationMetadata(appName: $appName, version: $version, '
      'buildNumber: $buildNumber)';
}
