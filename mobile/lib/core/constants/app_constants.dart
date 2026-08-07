import 'package:sports_gurukul/core/constants/api_constants.dart';

/// Application-wide, non-domain constants.
///
/// Values here are shared across the whole app. They never encode
/// environment-specific secrets (see `core/config`) or API shapes (see
/// [ApiConstants]); they describe the product itself.
abstract final class AppConstants {
  /// The display name of the application.
  static const String appName = 'Sports Gurukul';

  /// Default locale used when the device locale is not supported.
  static const String defaultLocale = 'en';

  /// Locales the application localizes for.
  static const List<String> supportedLocales = ['en', 'hi', 'mr'];

  /// Default page size for paginated lists.
  static const int defaultPageSize = 20;

  /// Upper bound accepted for page-size request parameters.
  static const int maxPageSize = 100;

  /// Default value used for unknown or missing scores.
  static const String defaultScorePlaceholder = '–';
}
