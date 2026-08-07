import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/buttons/gradient_button.dart';

/// Bottom navigation row shared by the create- and edit-academy wizards:
/// back, continue / submit.
class AcademyBottomBar extends StatelessWidget {
  const AcademyBottomBar({
    required this.submitting,
    required this.isLastStep,
    required this.onNext,
    this.onBack,
    this.submitLabel,
    super.key,
  });

  /// Whether a submit is in flight; disables the actions while true.
  final bool submitting;

  /// Whether the wizard is on its final step (renders the submit action).
  final bool isLastStep;

  /// Continue / submit handler.
  final VoidCallback onNext;

  /// Back handler; hidden when null.
  final VoidCallback? onBack;

  /// Overrides the submit-button label (e.g. "Save Changes" on edit). Only
  /// shown on the final step; earlier steps keep the "Continue" label.
  final String? submitLabel;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);

    return Row(
      children: <Widget>[
        if (onBack != null) ...<Widget>[
          Expanded(
            child: OutlinedButton(
              onPressed: submitting ? null : onBack,
              style: OutlinedButton.styleFrom(
                foregroundColor: AppColors.surface,
                side: const BorderSide(color: AppColors.glassBorderHi),
                minimumSize: const Size.fromHeight(56),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(AppRadius.xlarge),
                ),
              ),
              child: Text(l10n.academyBackButton),
            ),
          ),
          const SizedBox(width: AppSpacing.md),
        ],
        Expanded(
          flex: 2,
          child: GradientButton(
            label: isLastStep
                ? (submitLabel ?? l10n.academySubmitButton)
                : l10n.academyNextButton,
            icon: isLastStep
                ? Icons.check_rounded
                : Icons.arrow_forward_rounded,
            gradient: AppGradients.bluePurpleHorizontal,
            loading: submitting,
            onPressed: submitting ? null : onNext,
          ),
        ),
      ],
    );
  }
}
