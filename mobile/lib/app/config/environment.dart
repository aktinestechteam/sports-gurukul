import 'package:sports_gurukul/core/config/environment.dart' as core;

export 'package:sports_gurukul/core/config/environment.dart';
export 'package:sports_gurukul/core/config/flavor.dart';

/// Backwards-compatible alias for the build flavor.
///
/// Kept so existing references to the Sprint-0 `AppEnvironment` keep
/// compiling; new code should use `Flavor` and `Environment` from
/// `core/config`.
typedef AppEnvironment = core.Environment;
