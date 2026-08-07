/// Build flavors the application can be compiled for.
///
/// A flavor is selected at build time (via `--dart-define`) and drives which
/// configuration is loaded. The values match the Sprint-0 `AppEnvironment`
/// placeholder so existing references keep compiling.
enum Flavor {
  /// Local development builds.
  development,

  /// QA / integration-test builds.
  qa,

  /// User-acceptance-test builds.
  uat,

  /// Production builds.
  production,
}
