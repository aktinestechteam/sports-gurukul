/// Supported application environments.
///
/// Environment selection currently uses the development default
/// ([AppConfig.environment]). When the API layer lands (P003) the
/// environment will be resolved per build flavor
/// (sportsgurukul_dev/qa/uat/prod) and read from environment configuration.
/// The `dev/`, `qa/`, `uat/` and `production/` subfolders are the homes for
/// each environment's configuration.
///
/// Reference: docs/mobile/09-Implementation/01-Flutter-Project-Architecture.md
enum AppEnvironment {
  development,
  qa,
  uat,
  production,
}
