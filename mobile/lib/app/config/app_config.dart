import 'app_environment.dart';

/// Static application-wide configuration.
///
/// Values here are build-time constants. Environment-specific secrets
/// (API keys, tokens) must never be committed; they are injected through
/// environment configuration and secure storage in later sprints.
abstract final class AppConfig {
  /// Active environment. Resolved per build flavor from a later sprint;
  /// Sprint 0 defaults to development.
  static const AppEnvironment environment = AppEnvironment.development;

  static const bool isDebugMode =
      bool.fromEnvironment('dart.vm.product') == false;
}
