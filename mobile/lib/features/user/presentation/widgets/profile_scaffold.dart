import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:sports_gurukul/app/theme/app_animation.dart';
import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/app/theme/typography/app_typography.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/animations/entrance.dart';
import 'package:sports_gurukul/shared/animations/spring_press.dart';
import 'package:sports_gurukul/shared/text/gradient_text.dart';

/// Dark themed shell shared by the profile screens.
///
/// Follows the authentication screens' glass/gradient language (dark ink
/// background, frosted cards, gradient headings, gradient buttons) but renders
/// over a flat [AppColors.inkDeep] surface instead of the sports photo, so the
/// profile flow keeps its identity without a background image. A fixed header
/// carries the back button, brand wordmark and gradient screen title; each
/// page supplies its own scrollable [child].
class ProfileScaffold extends StatelessWidget {
  const ProfileScaffold({
    required this.title,
    required this.child,
    super.key,
    this.subtitle,
    this.trailing,
  });

  /// Screen heading rendered with the brand gradient.
  final String title;

  /// Optional supporting text under the heading.
  final String? subtitle;

  /// Optional widget pinned to the trailing edge of the header.
  final Widget? trailing;

  /// The scrollable screen body.
  final Widget child;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.inkDeep,
      body: SafeArea(
        bottom: false,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: <Widget>[
            _ProfileHeader(
              title: title,
              subtitle: subtitle,
              trailing: trailing,
            ),
            Expanded(child: child),
          ],
        ),
      ),
    );
  }
}

/// Branded header: back button, wordmark + gradient title, optional trailing.
class _ProfileHeader extends StatelessWidget {
  const _ProfileHeader({
    required this.title,
    required this.subtitle,
    this.trailing,
  });

  final String title;
  final String? subtitle;
  final Widget? trailing;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Entrance(
      duration: AppAnimation.entrance,
      offset: const Offset(0, 0.06),
      child: Padding(
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.xl,
          AppSpacing.lg,
          AppSpacing.xl,
          AppSpacing.lg,
        ),
        child: Row(
          children: <Widget>[
            const _BackButton(),
            const SizedBox(width: AppSpacing.md),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: <Widget>[
                  Text(
                    l10n.appTitle.toUpperCase(),
                    style: const TextStyle(
                      fontSize: AppTypography.caption,
                      fontWeight: AppTypography.semiBold,
                      letterSpacing: 1.6,
                      color: AppColors.loginSubtitle,
                    ),
                  ),
                  const SizedBox(height: AppSpacing.xs),
                  GradientText(
                    title,
                    gradient: AppGradients.bluePurple,
                    style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                      fontWeight: FontWeight.w800,
                      height: 1.15,
                    ),
                  ),
                  if (subtitle != null) ...<Widget>[
                    const SizedBox(height: AppSpacing.xs),
                    Text(
                      subtitle!,
                      style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: AppColors.loginSubtitle,
                        fontWeight: FontWeight.w500,
                        height: 1.3,
                      ),
                    ),
                  ],
                ],
              ),
            ),
            if (trailing != null) ...<Widget>[
              const SizedBox(width: AppSpacing.md),
              trailing!,
            ],
          ],
        ),
      ),
    );
  }
}

/// Frosted circular back button; hidden when there is nothing to pop to.
class _BackButton extends StatelessWidget {
  const _BackButton();

  @override
  Widget build(BuildContext context) {
    final canPop = context.canPop();
    if (!canPop) {
      return const SizedBox(width: 44);
    }
    return Tooltip(
      message: MaterialLocalizations.of(context).backButtonTooltip,
      child: SpringPress(
        onPressed: () {
          if (context.canPop()) {
            context.pop();
          }
        },
        scaleDown: 0.9,
        child: Container(
          width: 44,
          height: 44,
          decoration: BoxDecoration(
            shape: BoxShape.circle,
            color: AppColors.glassFillDark,
            border: Border.all(color: AppColors.whiteBorder),
          ),
          child: const Icon(
            Icons.arrow_back_ios_new_rounded,
            color: AppColors.surface,
            size: 18,
          ),
        ),
      ),
    );
  }
}
