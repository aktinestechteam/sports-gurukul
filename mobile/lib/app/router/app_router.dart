import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../features/dashboard/presentation/pages/dashboard_page.dart';
import '../bootstrap/splash_page.dart';
import 'route_names.dart';
import 'route_paths.dart';

/// Application-level [GoRouter] configuration.
///
/// Splash is the entry route. Authentication, role-based routing, deep
/// linking and route guards are delivered in later sprints; the placeholder
/// [RouteGuards] and [ShellRoutes] members exist to lock the shape in.
final appRouterProvider = Provider<GoRouter>((_) {
  return GoRouter(
    initialLocation: RoutePaths.splash,
    routes: <RouteBase>[
      GoRoute(
        name: RouteNames.splash,
        path: RoutePaths.splash,
        builder: (_, __) => const SplashPage(),
      ),
      GoRoute(
        name: RouteNames.dashboard,
        path: RoutePaths.dashboard,
        builder: (_, __) => const DashboardPage(),
      ),
    ],
  );
});
