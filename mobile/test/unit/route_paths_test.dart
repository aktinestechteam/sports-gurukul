import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/app/router/route_names.dart';
import 'package:sports_gurukul/app/router/route_paths.dart';

void main() {
  group('RoutePaths', () {
    test('splash is the initial route', () {
      expect(RoutePaths.splash, '/');
    });

    test('dashboard path is snake_case and prefixed', () {
      expect(RoutePaths.dashboard, '/dashboard');
    });
  });

  group('RouteNames', () {
    test('names mirror paths', () {
      expect(RouteNames.splash, 'splash');
      expect(RouteNames.dashboard, 'dashboard');
    });
  });
}
