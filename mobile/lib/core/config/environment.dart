/// Runtime environment classification.
///
/// Coarser than `Flavor`: several flavors can map to the same environment.
/// [Environment] is used to gate behaviors such as analytics, crash
/// reporting and feature flags. See `BuildEnvironment.fromFlavor` for the
/// canonical flavor-to-environment mapping.
enum Environment {
  /// Local development and feature work.
  development,

  /// QA and UAT builds.
  staging,

  /// Live production builds.
  production,
}
