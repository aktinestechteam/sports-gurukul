import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/animations/entrance.dart';
import 'package:sports_gurukul/shared/buttons/gradient_button.dart';
import 'package:sports_gurukul/shared/cards/glass_card.dart';
import 'package:sports_gurukul/shared/layouts/aurora_background.dart';

/// Navigation placeholder for the Create Academy / Join Academy flows.
///
/// The academy features are delivered in a later sprint; this screen exists
/// so the onboarding navigation can point at real routes today. It renders a
/// gradient icon chip, the localized [title]/[message] and a
/// [GradientButton] that returns to the dashboard.
class AcademyFlowPlaceholder extends StatelessWidget {
  const AcademyFlowPlaceholder({
    required this.icon,
    required this.gradient,
    required this.title,
    required this.message,
    required this.onBack,
    super.key,
  });

  final IconData icon;
  final Gradient gradient;
  final String title;
  final String message;
  final VoidCallback onBack;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Scaffold(
      backgroundColor: Colors.transparent,
      body: AuroraBackground(
        child: SafeArea(
          bottom: false,
          child: SingleChildScrollView(
            physics: const BouncingScrollPhysics(),
            padding: const EdgeInsets.all(AppSpacing.xl),
            child: Entrance(
              child: GlassCard(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: <Widget>[
                    Container(
                      width: 64,
                      height: 64,
                      decoration: BoxDecoration(
                        shape: BoxShape.circle,
                        gradient: gradient,
                      ),
                      child: Icon(icon, color: AppColors.surface, size: 30),
                    ),
                    const SizedBox(height: AppSpacing.lg),
                    Text(
                      title,
                      style: Theme.of(context).textTheme.headlineSmall
                          ?.copyWith(
                            color: AppColors.surface,
                            fontWeight: FontWeight.w800,
                          ),
                    ),
                    const SizedBox(height: AppSpacing.sm),
                    Text(
                      message,
                      style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                        color: AppColors.grey300,
                        height: 1.4,
                      ),
                    ),
                    const SizedBox(height: AppSpacing.xl),
                    GradientButton(
                      label: l10n.academyBackToDashboard,
                      icon: Icons.space_dashboard_rounded,
                      onPressed: onBack,
                    ),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
