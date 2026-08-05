import 'package:sports_gurukul/features/onboarding/domain/entities/application_session.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_role.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_state.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

/// A resolved profile-badge presentation: the localized label plus the
/// [UserRole] used only for the accent tint.
typedef UserStateBadge = ({String? label, UserRole? role});

/// Resolves the profile role badge for [session].
///
/// The badge is driven by the resolved [UserState], never by the raw backend
/// roles: a brand-new account (default `Athlete` registration role, no academy
/// association) is labelled "New User" with a neutral tint until a business
/// role is explicitly returned by the backend. This is the single source of
/// truth for role-badge labels used across the welcome screen, the limited
/// new-user dashboard and the full dashboard.
UserStateBadge resolveUserStateBadge(
  AppLocalizations l10n,
  ApplicationSession session,
) {
  final state = session.userState;
  final label = switch (state) {
    UserState.newUser => l10n.roleLabelNewUser,
    UserState.pendingApproval => l10n.roleLabelPendingApproval,
    UserState.academyMember => l10n.roleLabelMember,
    UserState.academyAdmin => l10n.roleLabelAcademyAdmin,
    UserState.coach => l10n.roleLabelCoach,
    UserState.athlete => l10n.roleLabelAthlete,
    UserState.systemAdmin => l10n.roleLabelSystemAdmin,
    UserState.unknown || UserState.unauthenticated => null,
  };
  final role = switch (state) {
    UserState.newUser ||
    UserState.pendingApproval ||
    UserState.unknown ||
    UserState.unauthenticated => null,
    _ => session.primaryRole,
  };
  return (label: label, role: role);
}
