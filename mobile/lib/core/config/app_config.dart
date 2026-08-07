import 'package:sports_gurukul/core/config/application_metadata.dart';
import 'package:sports_gurukul/core/config/build_environment.dart';
import 'package:sports_gurukul/core/config/environment.dart';
import 'package:sports_gurukul/core/config/flavor.dart';

/// Static accessor for the active build environment.
///
/// [configure] must be called once during startup with the environment
/// resolved from the build flavor. Before that, a development environment is
/// used so logging and other core facilities have a sane default.
abstract final class AppConfig {
  static final BuildEnvironment _defaultEnvironment =
      BuildEnvironment.fromFlavor(Flavor.development);

  static BuildEnvironment _current = _defaultEnvironment;

  /// The active [BuildEnvironment].
  static BuildEnvironment get current => _current;

  /// Replaces the active environment. Call once during startup.
  // ignore: use_setters_to_change_properties
  static void configure(BuildEnvironment environment) {
    _current = environment;
  }

  /// The active build flavor.
  static Flavor get flavor => _current.flavor;

  /// The active runtime environment classification.
  static Environment get environment => _current.environment;

  /// Whether the current environment is production.
  static bool get isProduction => _current.isProduction;

  /// Application metadata for the current environment.
  static ApplicationMetadata get metadata => _current.metadata;

  /// Compile-time debug flag, independent of the resolved flavor.
  static bool get isDebugMode => !const bool.fromEnvironment('dart.vm.product');
}
