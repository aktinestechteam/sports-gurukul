import 'package:flutter/foundation.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/utils/permission_utils.dart';

void main() {
  group('PermissionUtils', () {
    test('maps Android manifest keys', () {
      expect(
        PermissionUtils.androidManifestKey(PermissionType.camera),
        'android.permission.CAMERA',
      );
      expect(
        PermissionUtils.androidManifestKey(PermissionType.notification),
        'android.permission.POST_NOTIFICATIONS',
      );
    });

    test('maps iOS plist keys', () {
      expect(
        PermissionUtils.iosInfoPlistKey(PermissionType.camera),
        'NSCameraUsageDescription',
      );
      expect(
        PermissionUtils.iosInfoPlistKey(PermissionType.photoLibrary),
        'NSPhotoLibraryUsageDescription',
      );
    });

    test('storage is unsupported on iOS', () {
      expect(
        PermissionUtils.isSupportedOn(
          PermissionType.storage,
          TargetPlatform.android,
        ),
        isTrue,
      );
      expect(
        PermissionUtils.isSupportedOn(
          PermissionType.storage,
          TargetPlatform.iOS,
        ),
        isFalse,
      );
    });

    test('all permission types are declared for mobile platforms', () {
      for (final type in PermissionType.values) {
        expect(
          PermissionUtils.isSupportedOn(type, TargetPlatform.android),
          isTrue,
          reason: 'Android key missing for $type',
        );
      }
      expect(
        PermissionUtils.isSupportedOn(
          PermissionType.camera,
          TargetPlatform.linux,
        ),
        isFalse,
      );
    });
  });
}
