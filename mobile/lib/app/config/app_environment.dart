/// Supported application environments.
///
/// Environment selection currently uses the development default
/// ([AppConfig.environment]). When the API layer lands (P002) the
/// environment will be resolved per build flavor
/// (sportsgurukul_dev/qa/uat/prod) and read from environment configuration.
///
/// Reference: docs/mobile/09-Implementation/01-Flutter-Project-Architecture.md
enum AppEnvironment {
  development,
  qa,
  uat,
  production,
}
