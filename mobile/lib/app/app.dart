import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/app/router/app_router.dart';
import 'package:sports_gurukul/app/theme/material_theme/app_theme.dart';
import 'package:sports_gurukul/app/theme/material_theme/theme_mode_provider.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

/// Root widget of the Sports Gurukul application.
///
/// Composes the application shell: localization, theming and routing.
/// Business state and feature modules are wired in through Riverpod
/// providers as they are delivered in later sprints.
class SportsGurukulApp extends ConsumerWidget {
  const SportsGurukulApp({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final router = ref.watch(appRouterProvider);
    final themeMode = ref.watch(themeModeProvider);

    return MaterialApp.router(
      title: 'Sports Gurukul',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.light,
      darkTheme: AppTheme.dark,
      themeMode: themeMode,
      routerConfig: router,
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      onGenerateTitle: (context) => AppLocalizations.of(context).appTitle,
      locale: const Locale('en'),
    );
  }
}
