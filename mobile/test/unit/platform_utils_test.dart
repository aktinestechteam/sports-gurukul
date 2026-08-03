import 'package:flutter/foundation.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/utils/platform_utils.dart';

void main() {
  tearDown(() {
    debugDefaultTargetPlatformOverride = null;
  });

  group('PlatformUtils', () {
    test('detects Android', () {
      debugDefaultTargetPlatformOverride = TargetPlatform.android;
      expect(PlatformUtils.isAndroid, isTrue);
      expect(PlatformUtils.isMobile, isTrue);
      expect(PlatformUtils.isIOS, isFalse);
      expect(PlatformUtils.osName, 'android');
    });

    test('detects iOS', () {
      debugDefaultTargetPlatformOverride = TargetPlatform.iOS;
      expect(PlatformUtils.isIOS, isTrue);
      expect(PlatformUtils.isMobile, isTrue);
      expect(PlatformUtils.osName, 'ios');
    });

    test('detects desktop', () {
      debugDefaultTargetPlatformOverride = TargetPlatform.windows;
      expect(PlatformUtils.isWindows, isTrue);
      expect(PlatformUtils.isDesktop, isTrue);
      expect(PlatformUtils.isMobile, isFalse);
      expect(PlatformUtils.osName, 'windows');
    });

    test('osName maps every platform', () {
      for (final platform in TargetPlatform.values) {
        debugDefaultTargetPlatformOverride = platform;
        expect(PlatformUtils.osName, isNotEmpty);
      }
    });
  });
}
