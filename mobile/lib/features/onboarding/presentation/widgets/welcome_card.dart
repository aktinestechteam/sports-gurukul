import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/shared/cards/glass_card.dart';

/// Hero welcome message card shown to brand-new users.
///
/// Composes the [GlassCard] surface with a gradient icon chip and the
/// localized [title]/[subtitle]. Purely presentational.
class WelcomeCard extends StatelessWidget {
  const WelcomeCard({
    required this.title,
    required this.subtitle,
    super.key,
  });

  final String title;
  final String subtitle;

  @override
  Widget build(BuildContext context) {
    return GlassCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: <Widget>[
          Container(
            width: 64,
            height: 64,
            decoration: const BoxDecoration(
              shape: BoxShape.circle,
              gradient: AppGradients.aurora,
            ),
            child: const Icon(
              Icons.waving_hand_rounded,
              color: AppColors.surface,
              size: 32,
            ),
          ),
          const SizedBox(height: AppSpacing.lg),
          Text(
            title,
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
              color: AppColors.surface,
              fontWeight: FontWeight.w800,
            ),
          ),
          const SizedBox(height: AppSpacing.sm),
          Text(
            subtitle,
            style: Theme.of(context).textTheme.bodyMedium?.copyWith(
              color: AppColors.grey300,
              height: 1.4,
            ),
          ),
        ],
      ),
    );
  }
}
