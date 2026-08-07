import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/application_session.dart';
import 'package:sports_gurukul/features/onboarding/presentation/widgets/action_tile.dart';
import 'package:sports_gurukul/features/onboarding/presentation/widgets/profile_header.dart';
import 'package:sports_gurukul/features/onboarding/presentation/widgets/user_state_badge.dart';
import 'package:sports_gurukul/features/onboarding/presentation/widgets/welcome_card.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/animations/entrance.dart';

/// The identity header, welcome hero and the three onboarding path actions
/// shared by the welcome screen and the new-user dashboard.
///
/// Purely presentational: the three path callbacks and the logout callback are
/// wired by the owning page. Keeping the actions in one place stops the
/// welcome screen and the limited new-user dashboard from drifting apart.
class OnboardingActions extends StatelessWidget {
  const OnboardingActions({
    required this.session,
    required this.l10n,
    required this.onCreateAcademy,
    required this.onJoinAcademy,
    required this.onExplore,
    required this.onLogout,
    super.key,
  });

  final ApplicationSession session;
  final AppLocalizations l10n;
  final VoidCallback onCreateAcademy;
  final VoidCallback onJoinAcademy;
  final VoidCallback onExplore;
  final VoidCallback onLogout;

  @override
  Widget build(BuildContext context) {
    final user = session.currentUser;
    final badge = resolveUserStateBadge(l10n, session);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: <Widget>[
        Entrance(
          child: ProfileHeader(
            currentUser: user,
            roleLabel: badge.label ?? l10n.roleLabelMember,
            roleBadgeRole: badge.role,
            logoutLabel: l10n.authLogout,
            onLogout: onLogout,
          ),
        ),
        const SizedBox(height: AppSpacing.xxl),
        Entrance(
          delay: const Duration(milliseconds: 120),
          child: WelcomeCard(
            title: l10n.welcomeTitle,
            subtitle: l10n.welcomeSubtitle,
          ),
        ),
        const SizedBox(height: AppSpacing.xxxl),
        Entrance(
          delay: const Duration(milliseconds: 200),
          child: ActionTile(
            icon: Icons.school_rounded,
            title: l10n.welcomeCreateAcademy,
            subtitle: l10n.welcomeCreateAcademySubtitle,
            gradient: AppGradients.ocean,
            onPressed: onCreateAcademy,
          ),
        ),
        const SizedBox(height: AppSpacing.md),
        Entrance(
          delay: const Duration(milliseconds: 260),
          child: ActionTile(
            icon: Icons.groups_rounded,
            title: l10n.welcomeJoinAcademy,
            subtitle: l10n.welcomeJoinAcademySubtitle,
            gradient: AppGradients.emerald,
            onPressed: onJoinAcademy,
          ),
        ),
        const SizedBox(height: AppSpacing.md),
        Entrance(
          delay: const Duration(milliseconds: 320),
          child: ActionTile(
            icon: Icons.explore_rounded,
            title: l10n.welcomeExplore,
            subtitle: l10n.welcomeExploreSubtitle,
            gradient: AppGradients.primary,
            onPressed: onExplore,
          ),
        ),
      ],
    );
  }
}
