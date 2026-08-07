import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/onboarding/presentation/providers/onboarding_controller.dart';
import 'package:sports_gurukul/features/onboarding/presentation/widgets/onboarding_actions.dart';
import 'package:sports_gurukul/features/onboarding/presentation/widgets/onboarding_states.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/layouts/aurora_background.dart';

/// Onboarding path shown to brand-new users after login.
///
/// Watches the onboarding lifecycle and renders the three navigation-only
/// actions (Create My Academy / Join Existing Academy / Explore Application).
/// Choosing any action completes onboarding; Create and Join hand off to their
/// placeholder routes, Explore continues to the (limited) dashboard. The
/// target academy flows are delivered in a later sprint.
class WelcomePage extends ConsumerWidget {
  const WelcomePage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(onboardingControllerProvider);

    final content = switch (state) {
      OnboardingLoading() => OnboardingLoadingState(
        message: l10n.welcomeLoading,
      ),
      OnboardingIdle() => OnboardingEmptyState(
        message: l10n.welcomeEmptyMessage,
      ),
      OnboardingError(:final failure) => OnboardingErrorState(
        title: l10n.welcomeErrorTitle,
        message: _errorMessage(l10n, failure),
        retryLabel: l10n.welcomeRetry,
        onRetry: () =>
            ref.read(onboardingControllerProvider.notifier).refresh(),
      ),
      OnboardingResolved(:final session) ||
      OnboardingCompleted(:final session) => SingleChildScrollView(
        physics: const BouncingScrollPhysics(),
        padding: const EdgeInsets.only(bottom: AppSpacing.xxxl),
        child: OnboardingActions(
          session: session,
          l10n: l10n,
          onCreateAcademy: () =>
              _complete(context, ref, RoutePaths.createAcademy),
          onJoinAcademy: () => _complete(context, ref, RoutePaths.joinAcademy),
          onExplore: () => _complete(context, ref, RoutePaths.dashboard),
          onLogout: () => ref.read(authControllerProvider.notifier).logout(),
        ),
      ),
    };

    return Scaffold(
      backgroundColor: Colors.transparent,
      body: AuroraBackground(
        child: SafeArea(
          bottom: false,
          child: Padding(
            padding: const EdgeInsets.all(AppSpacing.xl),
            child: content,
          ),
        ),
      ),
    );
  }

  /// Marks onboarding as done and moves to [target].
  void _complete(BuildContext context, WidgetRef ref, String target) {
    ref.read(onboardingControllerProvider.notifier).completeOnboarding();
    context.go(target);
  }

  static String _errorMessage(
    AppLocalizations l10n,
    BaseFailure failure,
  ) => switch (failure) {
    NetworkFailure() => l10n.welcomeErrorsNetwork,
    ServerFailure() => l10n.welcomeErrorsServer,
    AuthenticationFailure() => l10n.welcomeErrorsSessionExpired,
    _ => l10n.welcomeErrorsUnknown,
  };
}
