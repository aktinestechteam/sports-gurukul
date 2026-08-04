import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

/// Entry screen of the application.
///
/// Displays branding for a minimum duration, then resolves the session state
/// (auto-login or signed out). The auth-aware router redirect hands off to
/// the dashboard or the login screen once the state is known.
class SplashPage extends ConsumerStatefulWidget {
  const SplashPage({super.key});

  @override
  ConsumerState<SplashPage> createState() => _SplashPageState();
}

class _SplashPageState extends ConsumerState<SplashPage> {
  static const Duration _minimumDisplayDuration = Duration(milliseconds: 1200);
  Timer? _restoreTimer;

  @override
  void initState() {
    super.initState();
    _restoreTimer = Timer(_minimumDisplayDuration, _restoreSession);
  }

  void _restoreSession() {
    if (!mounted) {
      return;
    }
    unawaited(ref.read(authControllerProvider.notifier).restoreSession());
  }

  @override
  void dispose() {
    _restoreTimer?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final colorScheme = Theme.of(context).colorScheme;

    return Scaffold(
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: <Widget>[
            Container(
              width: 96,
              height: 96,
              decoration: BoxDecoration(
                color: colorScheme.primaryContainer,
                shape: BoxShape.circle,
              ),
              child: Icon(
                Icons.sports_soccer,
                size: 48,
                color: colorScheme.primary,
              ),
            ),
            const SizedBox(height: AppSpacing.xl),
            Text(
              l10n.appTitle,
              style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                color: AppColors.primary500,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: AppSpacing.sm),
            Text(
              l10n.appSplashTagline,
              style: Theme.of(context).textTheme.bodyLarge,
            ),
            const SizedBox(height: AppSpacing.xxxl),
            SizedBox(
              width: 24,
              height: 24,
              child: CircularProgressIndicator(
                strokeWidth: 2.5,
                color: colorScheme.primary,
              ),
            ),
            const SizedBox(height: AppSpacing.lg),
            Text(
              l10n.appInitializing,
              style: Theme.of(context).textTheme.bodyMedium,
            ),
          ],
        ),
      ),
    );
  }
}
