import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/app_animation.dart';
import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/app/theme/typography/app_typography.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/animations/entrance.dart';
import 'package:sports_gurukul/shared/brand/brand_mark.dart';
import 'package:sports_gurukul/shared/cards/glass_card.dart';
import 'package:sports_gurukul/shared/layouts/sports_background.dart';
import 'package:sports_gurukul/shared/text/gradient_text.dart';

/// Branded scaffold shared by the authentication screens.
///
/// Follows the login screen layout exactly: a full-bleed [SportsBackground]
/// photo fades in, then a dark smoked [GlassCard] slides up carrying the
/// heading and form while the brand mark floats above and straddles the card's
/// top border. Forgot-password, reset-password and sign-up flows all render
/// inside this single themed shell.
class AuthScaffold extends StatelessWidget {
  const AuthScaffold({
    required this.title,
    required this.subtitle,
    required this.child,
    super.key,
  });

  /// Screen heading.
  final String title;

  /// Supporting text under the heading.
  final String subtitle;

  /// The form body.
  final Widget child;

  /// The floating brand mark's edge-to-edge layout size.
  static const double _brandSize = BrandMark.size;

  /// Shared entrance scale applied to both the card and the brand mark so the
  /// logo keeps the proportions defined in the approved mockup.
  static const double _brandScale = 0.75;

  @override
  Widget build(BuildContext context) {
    // How far the card drops inside the stack so the floating brand mark can
    // straddle its top border, half above and half below.
    const brandOverlap = _brandSize / 2;

    return Scaffold(
      backgroundColor: AppColors.inkDeep,
      body: Entrance(
        duration: AppAnimation.entrance,
        offset: Offset.zero,
        scaleFrom: 1,
        child: SportsBackground(
          child: SafeArea(
            child: LayoutBuilder(
              builder: (context, viewportConstraints) {
                // Centres the card vertically when it fits, while keeping the
                // page scrollable (so forms stay reachable) when the keyboard
                // is open or the viewport is short.
                final minContentHeight =
                    viewportConstraints.maxHeight > (AppSpacing.xxxl * 2)
                    ? viewportConstraints.maxHeight - (AppSpacing.xxxl * 2)
                    : 0.0;
                return SingleChildScrollView(
                  keyboardDismissBehavior:
                      ScrollViewKeyboardDismissBehavior.onDrag,
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppSpacing.xl,
                    vertical: AppSpacing.xxxl,
                  ),
                  child: ConstrainedBox(
                    constraints: BoxConstraints(
                      minHeight: minContentHeight,
                    ),
                    child: Center(
                      child: ConstrainedBox(
                        constraints: const BoxConstraints(maxWidth: 432),
                        // The card and the floating brand mark live in a stack
                        // so the logo can overlap the card's top border without
                        // clipping (see [BrandMark]).
                        child: Stack(
                          clipBehavior: Clip.none,
                          children: <Widget>[
                            // The card drops by half the logo's height so the
                            // mark sits evenly across the top border.
                            Padding(
                              padding: const EdgeInsets.only(top: brandOverlap),
                              child: Entrance(
                                duration: AppAnimation.entrance,
                                offset: const Offset(0, 0.08),
                                child: Transform.scale(
                                  scale: _brandScale,
                                  alignment: Alignment.topCenter,
                                  child: GlassCard(
                                    padding: const EdgeInsets.fromLTRB(
                                      AppSpacing.xxl,
                                      AppSpacing.huge,
                                      AppSpacing.xxl,
                                      AppSpacing.xxl,
                                    ),
                                    borderRadius: BorderRadius.circular(
                                      AppRadius.extraLarge,
                                    ),
                                    borderColor: AppColors.whiteBorder,
                                    child: Column(
                                      mainAxisSize: MainAxisSize.min,
                                      crossAxisAlignment:
                                          CrossAxisAlignment.stretch,
                                      children: <Widget>[
                                        _AuthHeader(
                                          title: title,
                                          subtitle: subtitle,
                                        ),
                                        const SizedBox(height: AppSpacing.xxl),
                                        child,
                                      ],
                                    ),
                                  ),
                                ),
                              ),
                            ),
                            // Floating brand mark, horizontally centred over
                            // the card with its centre on the top border so
                            // half sits above the glass and half overlaps it.
                            Positioned(
                              top: 0,
                              left: 0,
                              right: 0,
                              child: Center(
                                child: Transform.scale(
                                  scale: _brandScale,
                                  child: const BrandMark(),
                                ),
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                  ),
                );
              },
            ),
          ),
        ),
      ),
    );
  }
}

/// App title, screen heading and subtitle for an auth card.
class _AuthHeader extends StatelessWidget {
  const _AuthHeader({required this.title, required this.subtitle});

  final String title;
  final String subtitle;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final textTheme = Theme.of(context).textTheme;
    return Column(
      children: <Widget>[
        FittedBox(
          fit: BoxFit.scaleDown,
          child: Text(
            l10n.appTitle,
            style: const TextStyle(
              fontSize: AppTypography.displayMedium,
              fontWeight: AppTypography.bold,
              color: AppColors.surface,
              height: 1.1,
              letterSpacing: 0.5,
            ),
          ),
        ),
        const SizedBox(height: AppSpacing.sm),
        FittedBox(
          fit: BoxFit.scaleDown,
          child: GradientText(
            title,
            gradient: AppGradients.bluePurple,
            style: textTheme.headlineMedium?.copyWith(
              fontWeight: FontWeight.w800,
              height: 1.15,
            ),
          ),
        ),
        const SizedBox(height: AppSpacing.sm),
        Text(
          subtitle,
          textAlign: TextAlign.center,
          style: textTheme.bodyMedium?.copyWith(
            color: AppColors.loginSubtitle,
            fontWeight: FontWeight.w500,
            height: 1.4,
          ),
        ),
      ],
    );
  }
}
