import 'package:sports_gurukul/core/config/application_metadata.dart';
import 'package:sports_gurukul/core/config/environment.dart';
import 'package:sports_gurukul/core/config/flavor.dart';

/// Immutable description of the environment the app is running in.
///
/// A [BuildEnvironment] composes the resolved [Flavor], its derived
/// [Environment], a debug flag and application [metadata]. It is produced
/// once during startup and exposed through `AppConfig`.
class BuildEnvironment {
  const BuildEnvironment({
    required this.flavor,
    required this.environment,
    required this.isDebug,
    this.metadata = const ApplicationMetadata(),
  });

  /// Resolves the canonical [BuildEnvironment] for [flavor].
  factory BuildEnvironment.fromFlavor(
    Flavor flavor, {
    bool? isDebug,
    ApplicationMetadata? metadata,
  }) => BuildEnvironment(
    flavor: flavor,
    environment: switch (flavor) {
      Flavor.development => Environment.development,
      Flavor.qa || Flavor.uat => Environment.staging,
      Flavor.production => Environment.production,
    },
    isDebug: isDebug ?? flavor == Flavor.development,
    metadata: metadata ?? const ApplicationMetadata(),
  );

  /// The build flavor this environment was resolved from.
  final Flavor flavor;

  /// The derived runtime environment classification.
  final Environment environment;

  /// Whether this is a debug build of the application.
  final bool isDebug;

  /// Static metadata about the application itself.
  final ApplicationMetadata metadata;

  /// Whether this is a production environment.
  bool get isProduction => flavor == Flavor.production;

  /// Whether this is a staging (QA/UAT) environment.
  bool get isStaging => environment == Environment.staging;

  @override
  String toString() =>
      'BuildEnvironment(flavor: $flavor, environment: $environment, '
      'isDebug: $isDebug)';
}
