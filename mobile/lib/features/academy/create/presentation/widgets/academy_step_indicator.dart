import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

/// Horizontal step progress for the create-academy wizard.
///
/// Renders one dot per step (completed steps filled, the active step
/// highlighted) plus a "Step X of N · label" caption beneath.
class AcademyStepIndicator extends StatelessWidget {
  const AcademyStepIndicator({
    required this.step,
    required this.totalSteps,
    super.key,
  });

  final int step;
  final int totalSteps;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: <Widget>[
        Row(
          children: <Widget>[
            for (var index = 0; index < totalSteps; index++) ...<Widget>[
              _dot(active: index <= step),
              if (index < totalSteps - 1)
                Expanded(
                  child: Container(
                    height: 2,
                    margin: const EdgeInsets.symmetric(
                      horizontal: AppSpacing.xs,
                    ),
                    decoration: BoxDecoration(
                      color: index < step
                          ? AppColors.blue500
                          : AppColors.grey400.withValues(alpha: 0.5),
                      borderRadius: BorderRadius.circular(AppRadius.pill),
                    ),
                  ),
                ),
            ],
          ],
        ),
        const SizedBox(height: AppSpacing.sm),
        Text(
          l10n.academyStepIndicator(step + 1, totalSteps),
          style: Theme.of(context).textTheme.labelMedium?.copyWith(
            color: AppColors.loginSubtitle,
          ),
        ),
      ],
    );
  }

  Widget _dot({required bool active}) {
    return AnimatedContainer(
      duration: const Duration(milliseconds: 250),
      width: active ? 12 : 10,
      height: active ? 12 : 10,
      decoration: BoxDecoration(
        shape: BoxShape.circle,
        color: active ? AppColors.blue500 : Colors.transparent,
        border: Border.all(
          color: active ? AppColors.blue500 : AppColors.grey400,
          width: 2,
        ),
      ),
    );
  }
}
