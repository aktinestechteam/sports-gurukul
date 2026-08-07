import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_type.dart';
import 'package:sports_gurukul/features/academy/create/presentation/create_academy_draft.dart';
import 'package:sports_gurukul/features/academy/create/presentation/providers/academy_wizard_controller.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/cards/glass_card.dart';

/// Step 5: review the collected wizard data before submitting.
class AcademyReviewStep extends StatelessWidget {
  const AcademyReviewStep({
    required this.draft,
    required this.controller,
    super.key,
  });

  final CreateAcademyDraft draft;
  final AcademyWizardController controller;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: <Widget>[
        Text(
          l10n.academyReviewTitle,
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            color: AppColors.surface,
            fontWeight: FontWeight.w800,
          ),
        ),
        const SizedBox(height: AppSpacing.xs),
        Text(
          l10n.academyReviewSubtitle,
          style: Theme.of(context).textTheme.bodySmall?.copyWith(
            color: AppColors.grey300,
          ),
        ),
        const SizedBox(height: AppSpacing.lg),
        _ReviewSection(
          title: l10n.academyStepBasics,
          onEdit: () => controller.jumpTo(0),
          rows: <_Row>[
            _Row(l10n.academyNameLabel, draft.name),
            _Row(
              l10n.academyDescriptionLabel,
              _valueOrEmpty(l10n, draft.description),
            ),
            _Row(l10n.academyTypeLabel, _typeLabel(l10n, draft.academyType)),
            _Row(
              l10n.academySportsLabel,
              draft.sports.isEmpty
                  ? l10n.academyReviewNotProvided
                  : draft.sports.join(', '),
            ),
          ],
        ),
        const SizedBox(height: AppSpacing.lg),
        _ReviewSection(
          title: l10n.academyStepContact,
          onEdit: () => controller.jumpTo(1),
          rows: <_Row>[
            _Row(l10n.academyContactPersonLabel, draft.contactPerson),
            _Row(l10n.academyEmailLabel, draft.email),
            _Row(l10n.academyPhoneLabel, draft.phone),
            _Row(l10n.academyWebsiteLabel, _valueOrEmpty(l10n, draft.website)),
          ],
        ),
        const SizedBox(height: AppSpacing.lg),
        _ReviewSection(
          title: l10n.academyStepAddress,
          onEdit: () => controller.jumpTo(2),
          rows: <_Row>[
            _Row(l10n.academyCountryLabel, _valueOrEmpty(l10n, draft.country)),
            _Row(l10n.academyStateLabel, _valueOrEmpty(l10n, draft.state)),
            _Row(l10n.academyCityLabel, _valueOrEmpty(l10n, draft.city)),
            _Row(
              l10n.academyAddressLineLabel,
              _valueOrEmpty(l10n, draft.addressLine),
            ),
            _Row(
              l10n.academyPostalCodeLabel,
              _valueOrEmpty(l10n, draft.postalCode),
            ),
          ],
        ),
        const SizedBox(height: AppSpacing.lg),
        _ReviewSection(
          title: l10n.academyStepBranding,
          onEdit: () => controller.jumpTo(3),
          rows: <_Row>[
            _Row(
              l10n.academyLogoLabel,
              draft.logo == null
                  ? l10n.academyReviewNotProvided
                  : _fileName(draft.logo!.name),
            ),
            _Row(
              l10n.academyCoverLabel,
              draft.cover == null
                  ? l10n.academyReviewNotProvided
                  : _fileName(draft.cover!.name),
            ),
          ],
        ),
      ],
    );
  }

  static String _valueOrEmpty(AppLocalizations l10n, String value) =>
      value.trim().isEmpty ? l10n.academyReviewNotProvided : value;

  static String _fileName(String path) =>
      path.split(RegExp(r'[\\/]')).last;

  static String _typeLabel(AppLocalizations l10n, AcademyType? type) =>
      switch (type) {
        AcademyType.singleSport => l10n.academyTypeSingleSport,
        AcademyType.multiSport => l10n.academyTypeMultiSport,
        null => '',
      };
}

class _ReviewSection extends StatelessWidget {
  const _ReviewSection({
    required this.title,
    required this.onEdit,
    required this.rows,
  });

  final String title;
  final VoidCallback onEdit;
  final List<_Row> rows;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return GlassCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: <Widget>[
          Row(
            children: <Widget>[
              Expanded(
                child: Text(
                  title,
                  style: Theme.of(context).textTheme.titleSmall?.copyWith(
                    color: AppColors.surface,
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ),
              TextButton(
                onPressed: onEdit,
                child: Text(l10n.academyReviewEdit),
              ),
            ],
          ),
          const Divider(color: AppColors.glassBorderHi, height: 1),
          const SizedBox(height: AppSpacing.sm),
          for (final row in rows)
            Padding(
              padding: const EdgeInsets.symmetric(vertical: AppSpacing.xs),
              child: _ReviewRow(row: row),
            ),
        ],
      ),
    );
  }
}

class _ReviewRow extends StatelessWidget {
  const _ReviewRow({required this.row});

  final _Row row;

  @override
  Widget build(BuildContext context) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: <Widget>[
        SizedBox(
          width: 120,
          child: Text(
            row.label,
            style: Theme.of(context).textTheme.bodySmall?.copyWith(
              color: AppColors.grey300,
            ),
          ),
        ),
        Expanded(
          child: Text(
            row.value,
            style: Theme.of(context).textTheme.bodySmall?.copyWith(
              color: AppColors.surface,
              fontWeight: FontWeight.w600,
            ),
          ),
        ),
      ],
    );
  }
}

class _Row {
  const _Row(this.label, this.value);

  final String label;
  final String value;
}
