import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_role.dart';

/// A pill showing the user's primary [UserRole] with a role-tinted accent.
///
/// [label] is the localized role name; the tint comes from [role] so the
/// badge stays purely presentational (no business logic inside).
class RoleBadge extends StatelessWidget {
  const RoleBadge({required this.label, this.role, super.key});

  /// Localized role name to display.
  final String label;

  /// The role driving the accent color; unknown roles render neutral.
  final UserRole? role;

  @override
  Widget build(BuildContext context) {
    final accent = _accentForRole(role);
    return Container(
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.md,
        vertical: AppSpacing.xs,
      ),
      decoration: BoxDecoration(
        color: accent.withValues(alpha: 0.16),
        borderRadius: BorderRadius.circular(AppRadius.pill),
        border: Border.all(color: accent.withValues(alpha: 0.35)),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: <Widget>[
          Icon(Icons.workspace_premium_rounded, size: 14, color: accent),
          const SizedBox(width: AppSpacing.xs),
          Text(
            label,
            style: Theme.of(context).textTheme.labelMedium?.copyWith(
              color: accent,
              fontWeight: FontWeight.w700,
            ),
          ),
        ],
      ),
    );
  }

  static Color _accentForRole(UserRole? role) => switch (role) {
    UserRole.superAdmin || UserRole.admin => AppColors.information,
    UserRole.academy => AppColors.primary500,
    UserRole.coach => AppColors.cyan400,
    UserRole.athlete => AppColors.secondary,
    UserRole.parent => AppColors.pink400,
    UserRole.scout => AppColors.violet300,
    UserRole.sponsor => AppColors.accent,
    UserRole.aiAdministrator || null => AppColors.grey400,
  };
}
