import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/app/app.dart';
import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:go_router/go_router.dart';

/// Verifies the Sprint 0 bootstrap flow:
/// splash screen renders branding, then routes to the placeholder dashboard.
void main() {
  Future<void> pumpApp(WidgetTester tester) async {
    await tester.pumpWidget(
      const ProviderScope(
        child: SportsGurukulApp(),
      ),
    );
    await tester.pump();
  }

  testWidgets('shows splash branding on startup', (WidgetTester tester) async {
    await pumpApp(tester);

    expect(find.text('Sports Gurukul'), findsOneWidget);
    expect(find.text('Train • Compete • Excel'), findsOneWidget);
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
  });

  testWidgets('routes to placeholder dashboard after initialization', (
    WidgetTester tester,
  ) async {
    await pumpApp(tester);

    // Advance past the splash minimum display duration.
    await tester.pump(const Duration(seconds: 2));
    // Process the navigation and complete the route transition.
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(milliseconds: 400));

    expect(find.text('Project Initialized Successfully'), findsOneWidget);
    expect(find.byType(CircularProgressIndicator), findsNothing);
  });

  testWidgets('dashboard route is reachable directly', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(
      const ProviderScope(
        child: SportsGurukulApp(),
      ),
    );
    final BuildContext context = tester.element(find.byType(Scaffold).first);
    GoRouter.of(context).go(RoutePaths.dashboard);
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));

    expect(find.text('Project Initialized Successfully'), findsOneWidget);
  });
}
