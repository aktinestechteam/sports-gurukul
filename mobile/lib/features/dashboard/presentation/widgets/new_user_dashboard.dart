import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/application_session.dart';
import 'package:sports_gurukul/features/onboarding/presentation/widgets/onboarding_actions.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/layouts/aurora_background.dart';

/// The dashboard shown to logged-in users that are not yet part of an academy
/// and have no business role assigned (the "new user" state).
///
/// Per the onboarding spec such users must keep seeing the onboarding actions
/// instead of the normal dashboard: Create Academy / Join Academy navigate to
/// their placeholder flows, Explore stays on the dashboard (which remains in
/// this limited state until the profile gains an academy association).
class NewUserDashboard extends ConsumerWidget {
  const NewUserDashboard({required this.session, super.key});

  final ApplicationSession session;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    return Scaffold(
      backgroundColor: Colors.transparent,
      body: AuroraBackground(
        child: SafeArea(
          bottom: false,
          child: SingleChildScrollView(
            physics: const BouncingScrollPhysics(),
            padding: const EdgeInsets.fromLTRB(
              AppSpacing.xl,
              AppSpacing.md,
              AppSpacing.xl,
              AppSpacing.xxxl,
            ),
            child: OnboardingActions(
              session: session,
              l10n: l10n,
              onCreateAcademy: () => context.go(RoutePaths.createAcademy),
              onJoinAcademy: () => context.go(RoutePaths.joinAcademy),
              onExplore: () => context.go(RoutePaths.dashboard),
              onLogout: () =>
                  ref.read(authControllerProvider.notifier).logout(),
            ),
          ),
        ),
      ),
    );
  }
}
