import 'dart:ui';

import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/current_user.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_role.dart';
import 'package:sports_gurukul/features/onboarding/presentation/widgets/role_badge.dart';
import 'package:sports_gurukul/shared/animations/spring_press.dart';

/// Identity header for the welcome screen: avatar, name, email and role.
///
/// [roleLabel] is the localized primary-role name; [logoutLabel] is the
/// localized tooltip for the sign-out affordance; [onLogout] fires when the
/// user wants to sign out. Pure presentation — all values come in from the
/// resolved application session.
class ProfileHeader extends StatelessWidget {
  const ProfileHeader({
    required this.currentUser,
    required this.roleLabel,
    required this.logoutLabel,
    required this.onLogout,
    this.roleBadgeRole,
    super.key,
  });

  final CurrentUser currentUser;
  final String roleLabel;
  final String logoutLabel;
  final VoidCallback onLogout;

  /// The role used only for the badge accent tint; pass `null` for a neutral
  /// badge (e.g. new-user and pending-approval states).
  final UserRole? roleBadgeRole;

  @override
  Widget build(BuildContext context) {
    final initials = _initials(currentUser.fullName);
    return Row(
      children: <Widget>[
        Container(
          width: 56,
          height: 56,
          decoration: const BoxDecoration(
            shape: BoxShape.circle,
            gradient: AppGradients.ocean,
          ),
          alignment: Alignment.center,
          child: Text(
            initials,
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              color: AppColors.surface,
              fontWeight: FontWeight.w800,
            ),
          ),
        ),
        const SizedBox(width: AppSpacing.md),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: <Widget>[
              Text(
                currentUser.fullName,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  color: AppColors.surface,
                  fontWeight: FontWeight.w800,
                ),
              ),
              const SizedBox(height: AppSpacing.xs),
              Row(
                children: <Widget>[
                  Flexible(
                    child: Text(
                      currentUser.email,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                      style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: AppColors.grey300,
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: AppSpacing.xs),
              RoleBadge(
                role: roleBadgeRole ??
                    (currentUser.roles.isNotEmpty
                        ? currentUser.roles.first
                        : null),
                label: roleLabel,
              ),
            ],
          ),
        ),
        const SizedBox(width: AppSpacing.sm),
        _GlassCircle(
          size: 44,
          onTap: onLogout,
          tooltip: logoutLabel,
          child: const Icon(
            Icons.logout_rounded,
            color: AppColors.surface,
            size: 20,
          ),
        ),
      ],
    );
  }

  static String _initials(String name) {
    final parts = name.split(RegExp(r'\s+')).where((part) => part.isNotEmpty);
    final letters = parts.map((part) => part[0].toUpperCase());
    return letters.take(2).join();
  }
}

/// Circular glass surface; tappable with a tooltip.
class _GlassCircle extends StatelessWidget {
  const _GlassCircle({
    required this.size,
    required this.child,
    required this.onTap,
    required this.tooltip,
  });

  final double size;
  final Widget child;
  final VoidCallback onTap;
  final String tooltip;

  @override
  Widget build(BuildContext context) {
    return Tooltip(
      message: tooltip,
      child: SpringPress(
        onPressed: onTap,
        scaleDown: 0.9,
        child: ClipOval(
          child: BackdropFilter(
            filter: ImageFilter.blur(sigmaX: 10, sigmaY: 10),
            child: Container(
              width: size,
              height: size,
              decoration: const BoxDecoration(
                shape: BoxShape.circle,
                color: AppColors.glassFill,
                border: Border.fromBorderSide(
                  BorderSide(color: AppColors.glassBorderLo),
                ),
              ),
              alignment: Alignment.center,
              child: child,
            ),
          ),
        ),
      ),
    );
  }
}
