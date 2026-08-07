import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/app/theme/typography/app_typography.dart';
import 'package:sports_gurukul/shared/buttons/gradient_button.dart';

/// Reusable loading state.
class LoadingIndicator extends StatelessWidget {
  const LoadingIndicator({
    super.key,
    this.label,
    this.centered = true,
  });

  /// Optional text shown below the spinner.
  final String? label;

  /// When false the indicator renders inline (no full-screen centering).
  final bool centered;

  @override
  Widget build(BuildContext context) {
    final indicator = Column(
      mainAxisSize: MainAxisSize.min,
      children: <Widget>[
        const CircularProgressIndicator(),
        if (label != null) ...<Widget>[
          const SizedBox(height: AppSpacing.lg),
          Text(
            label!,
            textAlign: TextAlign.center,
            style: const TextStyle(
              fontSize: AppTypography.bodyMedium,
              color: AppColors.grey500,
            ),
          ),
        ],
      ],
    );
    if (!centered) {
      return indicator;
    }
    return Center(child: indicator);
  }
}

/// Reusable error state with a title, message and retry action.
class ErrorState extends StatelessWidget {
  const ErrorState({
    required this.title,
    required this.message,
    this.onRetry,
    this.retryLabel,
    this.action,
    super.key,
  });

  final String title;
  final String message;
  final VoidCallback? onRetry;
  final String? retryLabel;

  /// Optional primary action shown above the retry button.
  final Widget? action;

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(AppSpacing.xl),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: <Widget>[
            const Icon(
              Icons.error_outline,
              size: AppSpacing.huge,
              color: AppColors.danger,
            ),
            const SizedBox(height: AppSpacing.lg),
            Text(
              title,
              textAlign: TextAlign.center,
              style: const TextStyle(
                fontSize: AppTypography.headingSm,
                fontWeight: AppTypography.semiBold,
              ),
            ),
            const SizedBox(height: AppSpacing.sm),
            Text(
              message,
              textAlign: TextAlign.center,
              style: const TextStyle(
                fontSize: AppTypography.bodyMedium,
                color: AppColors.grey600,
                height: 1.4,
              ),
            ),
            if (action != null) ...<Widget>[
              const SizedBox(height: AppSpacing.xxl),
              action!,
            ],
            if (onRetry != null) ...<Widget>[
              const SizedBox(height: AppSpacing.lg),
              GradientButton(
                label: retryLabel ?? 'Retry',
                onPressed: onRetry,
              ),
            ],
          ],
        ),
      ),
    );
  }
}

/// Reusable empty state with an icon, title and optional subtitle.
class EmptyState extends StatelessWidget {
  const EmptyState({
    required this.icon,
    required this.title,
    this.subtitle,
    this.action,
    super.key,
  });

  final IconData icon;
  final String title;
  final String? subtitle;
  final Widget? action;

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(AppSpacing.xl),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: <Widget>[
            Icon(
              icon,
              size: AppSpacing.huge,
              color: AppColors.grey400,
            ),
            const SizedBox(height: AppSpacing.lg),
            Text(
              title,
              textAlign: TextAlign.center,
              style: const TextStyle(
                fontSize: AppTypography.headingSm,
                fontWeight: AppTypography.semiBold,
              ),
            ),
            if (subtitle != null) ...<Widget>[
              const SizedBox(height: AppSpacing.sm),
              Text(
                subtitle!,
                textAlign: TextAlign.center,
                style: const TextStyle(
                  fontSize: AppTypography.bodyMedium,
                  color: AppColors.grey600,
                ),
              ),
            ],
            if (action != null) ...<Widget>[
              const SizedBox(height: AppSpacing.xxl),
              action!,
            ],
          ],
        ),
      ),
    );
  }
}
