import 'package:device_info_plus/device_info_plus.dart';

/// Reads device metadata for diagnostics and device fingerprinting.
///
/// Wraps `device_info_plus` so call sites never depend on the concrete
/// package. [describe] returns the raw platform-specific key/value pairs
/// (`AndroidDeviceInfo`, `IosDeviceInfo`, ...) through the common
/// [BaseDeviceInfo].
class DeviceInfoService {
  DeviceInfoService({DeviceInfoPlugin? plugin})
    : _plugin = plugin ?? DeviceInfoPlugin();

  final DeviceInfoPlugin _plugin;

  /// Raw platform device information as key/value pairs.
  Future<Map<String, Object?>> describe() async =>
      (await _plugin.deviceInfo).data;
}
