import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:sports_gurukul/app/bootstrap/splash_page.dart';
import 'package:sports_gurukul/app/router/guards/route_guards.dart';
import 'package:sports_gurukul/app/router/navigation/shell_routes.dart';
import 'package:sports_gurukul/app/router/route_names.dart';
import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/features/dashboard/presentation/pages/dashboard_page.dart';

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
        builder: (_, _) => const SplashPage(),
      ),
      GoRoute(
        name: RouteNames.dashboard,
        path: RoutePaths.dashboard,
        builder: (_, _) => const DashboardPage(),
      ),
    ],
  );
});
