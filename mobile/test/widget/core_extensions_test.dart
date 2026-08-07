import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/extensions/context_extensions.dart';
import 'package:sports_gurukul/core/extensions/widget_extensions.dart';

Future<BuildContext> buildContext(
  WidgetTester tester, {
  Brightness brightness = Brightness.light,
}) async {
  late BuildContext context;
  await tester.pumpWidget(
    MaterialApp(
      theme: ThemeData(brightness: brightness),
      home: Builder(
        builder: (inner) {
          context = inner;
          return const Scaffold(body: SizedBox());
        },
      ),
    ),
  );
  return context;
}

void main() {
  group('BuildContextX', () {
    testWidgets('exposes theme and text styling', (tester) async {
      final context = await buildContext(tester);

      expect(context.theme, isA<ThemeData>());
      expect(context.textTheme, isA<TextTheme>());
      expect(context.colorScheme, isA<ColorScheme>());
      expect(context.mediaQuery, isA<MediaQueryData>());
      expect(context.screenWidth, greaterThan(0));
      expect(context.isDarkMode, isFalse);
    });

    testWidgets('detects dark mode', (tester) async {
      final context = await buildContext(tester, brightness: Brightness.dark);
      expect(context.isDarkMode, isTrue);
    });

    testWidgets('shows a snack bar', (tester) async {
      (await buildContext(tester)).showSnackBar('Saved');
      await tester.pump();
      expect(find.text('Saved'), findsOneWidget);
    });

    testWidgets('pops the current route', (tester) async {
      final context = await buildContext(tester);
      unawaited(
        Navigator.of(context).push(
          MaterialPageRoute<void>(
            builder: (_) => const Scaffold(body: Text('second')),
          ),
        ),
      );
      await tester.pumpAndSettle();
      expect(find.text('second'), findsOneWidget);

      context.pop();
      await tester.pumpAndSettle();
      expect(find.text('second'), findsNothing);
    });
  });

  group('WidgetX', () {
    testWidgets('wraps widgets in common containers', (tester) async {
      await tester.pumpWidget(
        MaterialApp(
          home: Scaffold(
            body: Column(
              children: [
                const Text('a').paddingAll(8),
                const Text('b').center(),
                const Text('c').expanded(),
                const Text('d').visible(visible: false),
                const Text('e').onTap(() {}),
                const Text('f').withTooltip('tip'),
                const Text('g').marginAll(4),
              ],
            ),
          ),
        ),
      );

      expect(
        find.ancestor(of: find.text('a'), matching: find.byType(Padding)),
        findsWidgets,
      );
      expect(
        find.ancestor(of: find.text('b'), matching: find.byType(Center)),
        findsOneWidget,
      );
      expect(
        find.ancestor(of: find.text('c'), matching: find.byType(Expanded)),
        findsOneWidget,
      );
      expect(find.text('d'), findsNothing);
      expect(
        find.ancestor(
          of: find.text('e'),
          matching: find.byType(GestureDetector),
        ),
        findsOneWidget,
      );
      expect(
        find.ancestor(of: find.text('f'), matching: find.byType(Tooltip)),
        findsOneWidget,
      );
      expect(
        find.ancestor(of: find.text('g'), matching: find.byType(Container)),
        findsWidgets,
      );
    });

    testWidgets('visible controls visibility', (tester) async {
      await tester.pumpWidget(
        MaterialApp(
          home: Scaffold(
            body: const Text('shown').visible(visible: false),
          ),
        ),
      );
      final visibility = tester.widget<Visibility>(find.byType(Visibility));
      expect(visibility.visible, isFalse);
    });

    testWidgets('sliverToBoxAdapter builds a sliver', (tester) async {
      await tester.pumpWidget(
        MaterialApp(
          home: Scaffold(
            body: CustomScrollView(
              slivers: [const Text('head').sliverToBoxAdapter()],
            ),
          ),
        ),
      );
      expect(find.byType(SliverToBoxAdapter), findsOneWidget);
    });

    testWidgets('tooltip carries the message', (tester) async {
      await tester.pumpWidget(
        MaterialApp(
          home: Scaffold(
            body: const Text('f').withTooltip('tip'),
          ),
        ),
      );
      final tooltip = tester.widget<Tooltip>(find.byType(Tooltip));
      expect(tooltip.message, 'tip');
    });
  });
}
