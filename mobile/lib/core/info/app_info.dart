import 'package:package_info_plus/package_info_plus.dart';

/// Reads application metadata (name, version, build number).
///
/// Wraps `package_info_plus` so call sites never depend on the concrete
/// package. Used for "About" screens, version gates and diagnostics.
class AppInfo {
  /// Loads the metadata of the running application.
  Future<PackageInfo> load() => PackageInfo.fromPlatform();
}
