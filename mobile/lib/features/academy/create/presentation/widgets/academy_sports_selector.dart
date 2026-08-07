import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/core/validators/validation_error.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_sports.dart';
import 'package:sports_gurukul/features/academy/create/presentation/academy_create_messages.dart';
import 'package:sports_gurukul/features/academy/create/presentation/create_academy_draft.dart';
import 'package:sports_gurukul/features/academy/create/presentation/providers/academy_wizard_controller.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

/// Multi-select sports chips for the basic-information step.
///
/// Renders inside a [Form] so the at-least-one check runs with the step
/// validation; selections are mirrored into the wizard draft. In [readOnly]
/// mode (edit flow) the selected sports are shown as static chips instead.
class AcademySportsSelector extends StatelessWidget {
  const AcademySportsSelector({
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
          Text(
            l10n.academySportsLabel,
            style: Theme.of(context).textTheme.labelLarge?.copyWith(
              color: AppColors.loginSubtitle,
              fontWeight: FontWeight.w600,
            ),
          ),
          const SizedBox(height: AppSpacing.sm),
          if (draft.sports.isEmpty)
            Text(
              l10n.academyReviewNotProvided,
              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                color: AppColors.grey300,
              ),
            )
          else
            Wrap(
              spacing: AppSpacing.sm,
              runSpacing: AppSpacing.sm,
              children: <Widget>[
                for (final sport in draft.sports)
                  Chip(
                    label: Text(sport),
                    labelStyle: const TextStyle(
                      color: AppColors.surface,
                      fontWeight: FontWeight.w600,
                    ),
                    backgroundColor: AppColors.blue600,
                    side: const BorderSide(color: AppColors.blue500),
                  ),
              ],
            ),
        ],
      );
    }

    return FormField<List<String>>(
      initialValue: draft.sports,
      validator: (value) => AcademyCreateMessages.validation(
        context,
        value == null || value.isEmpty
            ? const ValidationError('academy.validation.sport')
            : null,
      ),
      builder: (field) {
        final selected = field.value ?? const <String>[];
        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: <Widget>[
            Text(
              l10n.academySportsLabel,
              style: Theme.of(context).textTheme.labelLarge?.copyWith(
                color: AppColors.loginSubtitle,
                fontWeight: FontWeight.w600,
              ),
            ),
            const SizedBox(height: AppSpacing.sm),
            Wrap(
              spacing: AppSpacing.sm,
              runSpacing: AppSpacing.sm,
              children: AcademySports.catalog.map((sport) {
                final isSelected = selected.contains(sport);
                return FilterChip(
                  label: Text(sport),
                  selected: isSelected,
                  checkmarkColor: AppColors.surface,
                  labelStyle: TextStyle(
                    color: isSelected
                        ? AppColors.surface
                        : AppColors.surfaceVariant,
                    fontWeight: FontWeight.w600,
                  ),
                  backgroundColor: AppColors.loginFieldFill,
                  selectedColor: AppColors.blue600,
                  side: BorderSide(
                    color: isSelected
                        ? AppColors.blue500
                        : AppColors.grey400.withValues(alpha: 0.7),
                  ),
                  onSelected: (nowSelected) {
                    final updated = <String>[...selected];
                    if (nowSelected) {
                      if (!updated.contains(sport)) {
                        updated.add(sport);
                      }
                    } else {
                      updated.remove(sport);
                    }
                    field.didChange(updated);
                    controller.updateBasic(sports: updated);
                  },
                );
              }).toList(growable: false),
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
}
