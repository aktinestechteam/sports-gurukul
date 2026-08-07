import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/core/validators/validation_error.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_type.dart';
import 'package:sports_gurukul/features/academy/create/presentation/academy_create_messages.dart';
import 'package:sports_gurukul/features/academy/create/presentation/create_academy_draft.dart';
import 'package:sports_gurukul/features/academy/create/presentation/providers/academy_wizard_controller.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

/// Single-/multi-sport selection for the basic-information step.
///
/// Renders inside a [Form] so the required check runs with the step
/// validation; the choice is mirrored into the wizard draft. In [readOnly]
/// mode (edit flow) the selected type is shown as a static card instead.
class AcademyTypeSelector extends StatelessWidget {
  const AcademyTypeSelector({
    required this.draft,
    required this.controller,
    this.readOnly = false,
    super.key,
  });

  final CreateAcademyDraft draft;
  final AcademyWizardController controller;
  final bool readOnly;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    if (readOnly) {
      return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: <Widget>[
          _label(context, l10n.academyTypeLabel),
          const SizedBox(height: AppSpacing.sm),
          if (draft.academyType == null)
            Text(
              l10n.academyReviewNotProvided,
              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                color: AppColors.grey300,
              ),
            )
          else
            _TypeCard(
              title: _title(l10n, draft.academyType!),
              hint: _hint(l10n, draft.academyType!),
              icon: draft.academyType == AcademyType.singleSport
                  ? Icons.stars_rounded
                  : Icons.grid_view_rounded,
              selected: true,
              onTap: null,
            ),
        ],
      );
    }

    return FormField<AcademyType?>(
      initialValue: draft.academyType,
      validator: (value) => AcademyCreateMessages.validation(
        context,
        value == null ? const ValidationError('validation.required') : null,
      ),
      builder: (field) {
        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: <Widget>[
            _label(context, l10n.academyTypeLabel),
            const SizedBox(height: AppSpacing.sm),
            Row(
              children: <Widget>[
                Expanded(
                  child: _TypeCard(
                    title: l10n.academyTypeSingleSport,
                    hint: l10n.academyTypeSingleSportHint,
                    icon: Icons.stars_rounded,
                    selected: field.value == AcademyType.singleSport,
                    onTap: () {
                      field.didChange(AcademyType.singleSport);
                      controller.updateBasic(
                        academyType: AcademyType.singleSport,
                      );
                    },
                  ),
                ),
                const SizedBox(width: AppSpacing.md),
                Expanded(
                  child: _TypeCard(
                    title: l10n.academyTypeMultiSport,
                    hint: l10n.academyTypeMultiSportHint,
                    icon: Icons.grid_view_rounded,
                    selected: field.value == AcademyType.multiSport,
                    onTap: () {
                      field.didChange(AcademyType.multiSport);
                      controller.updateBasic(
                        academyType: AcademyType.multiSport,
                      );
                    },
                  ),
                ),
              ],
            ),
            if (field.hasError)
              Padding(
                padding: const EdgeInsets.only(
                  top: AppSpacing.xs,
                  left: AppSpacing.lg,
                ),
                child: Text(
                  field.errorText ?? '',
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: AppColors.danger,
                  ),
                ),
              ),
          ],
        );
      },
    );
  }

  static String _title(AppLocalizations l10n, AcademyType type) =>
      switch (type) {
        AcademyType.singleSport => l10n.academyTypeSingleSport,
        AcademyType.multiSport => l10n.academyTypeMultiSport,
      };

  static String _hint(AppLocalizations l10n, AcademyType type) =>
      switch (type) {
        AcademyType.singleSport => l10n.academyTypeSingleSportHint,
        AcademyType.multiSport => l10n.academyTypeMultiSportHint,
      };

  static Widget _label(BuildContext context, String text) => Padding(
    padding: const EdgeInsets.only(left: AppSpacing.xs),
    child: Text(
      text,
      style: Theme.of(context).textTheme.labelLarge?.copyWith(
        color: AppColors.loginSubtitle,
        fontWeight: FontWeight.w600,
      ),
    ),
  );
}

/// A tappable option card with a selection ring; `onTap: null` renders it
/// as a static read-only selection.
class _TypeCard extends StatelessWidget {
  const _TypeCard({
    required this.title,
    required this.hint,
    required this.icon,
    required this.selected,
    required this.onTap,
  });

  final String title;
  final String hint;
  final IconData icon;
  final bool selected;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    return InkWell(
      borderRadius: BorderRadius.circular(AppRadius.large),
      onTap: onTap,
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        padding: const EdgeInsets.all(AppSpacing.lg),
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(AppRadius.large),
          gradient: selected
              ? AppGradients.bluePurpleHorizontal
              : AppGradients.glassHighlight,
          border: Border.all(
            color: selected ? AppColors.blue500 : AppColors.glassBorderHi,
            width: selected ? 2 : 1,
          ),
          boxShadow: selected
              ? const <BoxShadow>[
                  BoxShadow(
                    color: AppColors.blue600,
                    blurRadius: 18,
                    offset: Offset(0, 6),
                  ),
                ]
              : null,
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: <Widget>[
            Row(
              children: <Widget>[
                Icon(
                  icon,
                  color: selected ? AppColors.surface : AppColors.blue500,
                  size: 20,
                ),
                const Spacer(),
                if (selected)
                  const Icon(
                    Icons.check_circle_rounded,
                    color: AppColors.surface,
                    size: 20,
                  ),
              ],
            ),
            const SizedBox(height: AppSpacing.sm),
            Text(
              title,
              style: Theme.of(context).textTheme.titleSmall?.copyWith(
                color: AppColors.surface,
                fontWeight: FontWeight.w700,
              ),
            ),
            const SizedBox(height: AppSpacing.xs),
            Text(
              hint,
              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                color: selected
                    ? AppColors.surface.withValues(alpha: 0.85)
                    : AppColors.grey300,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
