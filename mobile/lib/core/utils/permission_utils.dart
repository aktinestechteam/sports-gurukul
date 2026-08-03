import 'package:flutter/foundation.dart';

/// Permission types the app may request at runtime.
enum PermissionType {
  camera,
  microphone,
  location,
  photoLibrary,
  notification,
  contacts,
  calendar,
  storage,
}

/// Dependency-free helpers for runtime permission declarations.
///
/// This maps [PermissionType] to the platform declaration keys needed in
/// `AndroidManifest.xml` and `Info.plist`. Actual permission requests are
/// handled by the permissions layer (a later sprint); these helpers keep
/// declaration metadata in one place and are fully unit-testable.
abstract final class PermissionUtils {
  static const Map<PermissionType, String> _androidKeys = {
    PermissionType.camera: 'android.permission.CAMERA',
    PermissionType.microphone: 'android.permission.RECORD_AUDIO',
    PermissionType.location: 'android.permission.ACCESS_FINE_LOCATION',
    PermissionType.photoLibrary: 'android.permission.READ_MEDIA_IMAGES',
    PermissionType.notification: 'android.permission.POST_NOTIFICATIONS',
    PermissionType.contacts: 'android.permission.READ_CONTACTS',
    PermissionType.calendar: 'android.permission.READ_CALENDAR',
    PermissionType.storage: 'android.permission.READ_EXTERNAL_STORAGE',
  };

  static const Map<PermissionType, String> _iosKeys = {
    PermissionType.camera: 'NSCameraUsageDescription',
    PermissionType.microphone: 'NSMicrophoneUsageDescription',
    PermissionType.location: 'NSLocationWhenInUseUsageDescription',
    PermissionType.photoLibrary: 'NSPhotoLibraryUsageDescription',
    PermissionType.contacts: 'NSContactsUsageDescription',
    PermissionType.calendar: 'NSCalendarsUsageDescription',
  };

  /// The Android manifest key declaring [type], or `null` when the
  /// platform does not require one.
  static String? androidManifestKey(PermissionType type) => _androidKeys[type];

  /// The iOS `Info.plist` key declaring [type], or `null` when the platform
  /// does not require one.
  static String? iosInfoPlistKey(PermissionType type) => _iosKeys[type];

  /// Whether [type] is meaningful on [platform].
  static bool isSupportedOn(PermissionType type, TargetPlatform platform) {
    if (platform == TargetPlatform.android) {
      return _androidKeys.containsKey(type);
    }
    if (platform == TargetPlatform.iOS) {
      return _iosKeys.containsKey(type);
    }
    return false;
  }
}
