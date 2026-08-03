import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/config/app_config.dart';
import 'package:sports_gurukul/core/config/application_metadata.dart';
import 'package:sports_gurukul/core/config/build_environment.dart';
import 'package:sports_gurukul/core/config/environment.dart';
import 'package:sports_gurukul/core/config/flavor.dart';

void main() {
  tearDown(() {
    AppConfig.configure(BuildEnvironment.fromFlavor(Flavor.development));
  });

  group('BuildEnvironment', () {
    test('fromFlavor maps development', () {
      final env = BuildEnvironment.fromFlavor(Flavor.development);
      expect(env.environment, Environment.development);
      expect(env.isDebug, isTrue);
      expect(env.isProduction, isFalse);
      expect(env.isStaging, isFalse);
    });

    test('fromFlavor maps qa and uat to staging', () {
      expect(
        BuildEnvironment.fromFlavor(Flavor.qa).environment,
        Environment.staging,
      );
      expect(BuildEnvironment.fromFlavor(Flavor.qa).isStaging, isTrue);
      expect(
        BuildEnvironment.fromFlavor(Flavor.uat).environment,
        Environment.staging,
      );
    });

    test('fromFlavor maps production', () {
      final env = BuildEnvironment.fromFlavor(Flavor.production);
      expect(env.environment, Environment.production);
      expect(env.isDebug, isFalse);
      expect(env.isProduction, isTrue);
    });

    test('accepts an explicit debug flag and metadata', () {
      final env = BuildEnvironment.fromFlavor(
        Flavor.production,
        isDebug: true,
        metadata: const ApplicationMetadata(version: '1.2.3'),
      );
      expect(env.isDebug, isTrue);
      expect(env.metadata.version, '1.2.3');
    });
  });

  group('ApplicationMetadata', () {
    test('defaults to the app name and empty version', () {
      const metadata = ApplicationMetadata();
      expect(metadata.appName, 'Sports Gurukul');
      expect(metadata.version, '');
      expect(metadata.buildNumber, '');
    });
  });

  group('AppConfig', () {
    test('defaults to a development environment', () {
      expect(AppConfig.flavor, Flavor.development);
      expect(AppConfig.environment, Environment.development);
      expect(AppConfig.isProduction, isFalse);
    });

    test('reflects a configured environment', () {
      AppConfig.configure(BuildEnvironment.fromFlavor(Flavor.production));
      expect(AppConfig.flavor, Flavor.production);
      expect(AppConfig.environment, Environment.production);
      expect(AppConfig.isProduction, isTrue);
    });

    test('exposes metadata from the current environment', () {
      AppConfig.configure(
        BuildEnvironment.fromFlavor(
          Flavor.production,
          metadata: const ApplicationMetadata(version: '2.0.0'),
        ),
      );
      expect(AppConfig.metadata.version, '2.0.0');
    });

    test('isDebugMode reflects the compiled build', () {
      expect(AppConfig.isDebugMode, isTrue);
    });
  });
}
