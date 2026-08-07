import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/app/theme/app_animation.dart';
import 'package:sports_gurukul/app/theme/app_shadow.dart';
import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/app/theme/typography/app_typography.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/validators/email_validator.dart';
import 'package:sports_gurukul/core/validators/required_validator.dart';
import 'package:sports_gurukul/core/validators/validator.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/authentication/presentation/widgets/auth_messages.dart';
import 'package:sports_gurukul/features/authentication/presentation/widgets/auth_text_field.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/animations/entrance.dart';
import 'package:sports_gurukul/shared/brand/brand_mark.dart';
import 'package:sports_gurukul/shared/buttons/gradient_button.dart';
import 'package:sports_gurukul/shared/cards/glass_card.dart';
import 'package:sports_gurukul/shared/layouts/sports_background.dart';
import 'package:sports_gurukul/shared/text/gradient_text.dart';

/// Email/password sign-in screen.
///
/// Recreation of the approved login mockup: a full-bleed [SportsBackground]
/// photo fades in, then a frosted blue-tinted [GlassCard] slides up carrying
/// the title, form and actions, while the brand mark floats above and
/// straddles the card's top border. Auth logic, providers, navigation and
/// validation are untouched; only the presentation is driven by the design.
class LoginPage extends ConsumerStatefulWidget {
  const LoginPage({super.key});

  @override
  ConsumerState<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends ConsumerState<LoginPage> {
  /// The floating brand mark's edge-to-edge layout size.
  static const double _brandSize = BrandMark.size;

  /// Shared entrance scale applied to both the card and the brand mark so the
  /// logo keeps the proportions defined in the approved mockup.
  static const double _brandScale = 0.75;

  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _emailController;
  late final TextEditingController _passwordController;
  bool _submitting = false;
  bool _rememberMe = false;

  @override
  void initState() {
    super.initState();
    _emailController = TextEditingController();
    _passwordController = TextEditingController();
  }

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    FocusScope.of(context).unfocus();
    if (!_formKey.currentState!.validate()) {
      return;
    }
    setState(() => _submitting = true);
    final result = await ref
        .read(authControllerProvider.notifier)
        .login(
          email: _emailController.text.trim(),
          password: _passwordController.text,
        );
    if (!mounted) {
      return;
    }
    setState(() => _submitting = false);
    result.when(
      onSuccess: (_) {
        // Navigation to the dashboard is driven by the auth-aware redirect.
      },
      onFailure: _showFailure,
    );
  }

  void _showFailure(BaseFailure failure) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(AuthMessages.apiFailure(context, failure))),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);

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
                // page scrollable (so the sign-up link stays reachable) when
                // the keyboard is open or the viewport is short.
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
                        // clipping (see [_BrandMark]).
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
                                    child: AutofillGroup(
                                      child: Form(
                                        key: _formKey,
                                        child: Column(
                                          mainAxisSize: MainAxisSize.min,
                                          crossAxisAlignment:
                                              CrossAxisAlignment.stretch,
                                          children: <Widget>[
                                            const _LoginHeader(),
                                            const SizedBox(
                                              height: AppSpacing.xxl,
                                            ),
                                            AuthTextField(
                                              controller: _emailController,
                                              label: l10n.authLoginEmailLabel,
                                              icon: Icons.mail_outline,
                                              iconColor: AppColors.surface,
                                              labelColor:
                                                  AppColors.loginSubtitle,
                                              floatingLabelColor:
                                                  AppColors.blue500,
                                              textColor: AppColors.surface,
                                              fillColor:
                                                  AppColors.loginFieldFill,
                                              enabledBorderColor: AppColors
                                                  .grey400
                                                  .withValues(alpha: 0.7),
                                              focusedBorderColor:
                                                  AppColors.blue500,
                                              errorColor: AppColors.danger,
                                              keyboardType:
                                                  TextInputType.emailAddress,
                                              textInputAction:
                                                  TextInputAction.next,
                                              autofillHints: const <String>[
                                                AutofillHints.email,
                                              ],
                                              validator: (value) =>
                                                  AuthMessages.validation(
                                                    context,
                                                    const CompositeValidator<
                                                          String
                                                        >(<Validator<String>>[
                                                          RequiredValidator(),
                                                          EmailValidator(),
                                                        ])
                                                        .validate(value),
                                                  ),
                                            ),
                                            const SizedBox(
                                              height: AppSpacing.xxxxxl,
                                            ),
                                            AuthTextField(
                                              controller: _passwordController,
                                              label:
                                                  l10n.authLoginPasswordLabel,
                                              icon: Icons.lock_outline,
                                              iconColor: AppColors.surface,
                                              labelColor:
                                                  AppColors.loginSubtitle,
                                              floatingLabelColor:
                                                  AppColors.blue500,
                                              textColor: AppColors.surface,
                                              fillColor:
                                                  AppColors.loginFieldFill,
                                              enabledBorderColor: AppColors
                                                  .grey400
                                                  .withValues(alpha: 0.7),
                                              focusedBorderColor:
                                                  AppColors.blue500,
                                              errorColor: AppColors.danger,
                                              obscureText: true,
                                              textInputAction:
                                                  TextInputAction.done,
                                              autofillHints: const <String>[
                                                AutofillHints.password,
                                              ],
                                              validator: (value) =>
                                                  AuthMessages.validation(
                                                    context,
                                                    const RequiredValidator<
                                                          String
                                                        >()
                                                        .validate(value),
                                                  ),
                                              onFieldSubmitted: (_) =>
                                                  _submitting
                                                  ? null
                                                  : _submit(),
                                            ),
                                            const SizedBox(
                                              height: AppSpacing.xxl,
                                            ),
                                            _OptionsRow(
                                              rememberMe: _rememberMe,
                                              submitting: _submitting,
                                              onRememberChanged: (value) =>
                                                  setState(
                                                    () => _rememberMe = value,
                                                  ),
                                              onForgotPressed: _submitting
                                                  ? null
                                                  : () => context.go(
                                                      RoutePaths.forgotPassword,
                                                    ),
                                            ),
                                            const SizedBox(
                                              height: AppSpacing.xxxl,
                                            ),
                                            GradientButton(
                                              label: l10n.authLoginSubmit,
                                              gradient: AppGradients
                                                  .bluePurpleHorizontal,
                                              onPressed: _submitting
                                                  ? null
                                                  : _submit,
                                              loading: _submitting,
                                              shadows: AppShadow.glowPrimary,
                                              borderRadius:
                                                  BorderRadius.circular(
                                                    AppRadius.input,
                                                  ),
                                            ),
                                            const SizedBox(
                                              height: AppSpacing.xxl,
                                            ),
                                            _OrDivider(
                                              label: l10n.authLoginOr,
                                            ),
                                            const SizedBox(
                                              height: AppSpacing.xxl,
                                            ),
                                            _SignUpPrompt(
                                              onSignUpPressed: _submitting
                                                  ? null
                                                  : () => context.go(
                                                      RoutePaths.signUp,
                                                    ),
                                            ),
                                          ],
                                        ),
                                      ),
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

/// App title, welcome and subtitle for the login card.
///
/// The brand mark no longer lives here; it floats above the card and is
/// rendered by [BrandMark].
class _LoginHeader extends StatelessWidget {
  const _LoginHeader();

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
            l10n.authLoginTitle,
            gradient: AppGradients.bluePurple,
            style: textTheme.headlineMedium?.copyWith(
              fontWeight: FontWeight.w800,
              height: 1.15,
            ),
          ),
        ),
        const SizedBox(height: AppSpacing.sm),
        Text(
          l10n.authLoginSubtitle,
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

/// Remember-me checkbox (left) and forgot-password link (right).
class _OptionsRow extends StatelessWidget {
  const _OptionsRow({
    required this.rememberMe,
    required this.submitting,
    required this.onRememberChanged,
    required this.onForgotPressed,
  });

  final bool rememberMe;
  final bool submitting;
  final ValueChanged<bool> onRememberChanged;
  final VoidCallback? onForgotPressed;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final textTheme = Theme.of(context).textTheme;
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: <Widget>[
        Flexible(
          child: FittedBox(
            fit: BoxFit.scaleDown,
            alignment: Alignment.centerLeft,
            child: MergeSemantics(
              child: Row(
                mainAxisSize: MainAxisSize.min,
                children: <Widget>[
                  _RememberSwitch(
                    value: rememberMe,
                    onChanged: onRememberChanged,
                  ),
                  const SizedBox(width: AppSpacing.sm),
                  Text(
                    l10n.authLoginRememberMe,
                    style: textTheme.bodyMedium?.copyWith(
                      color: AppColors.surface,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
        Flexible(
          child: Align(
            alignment: Alignment.centerRight,
            child: InkWell(
              borderRadius: BorderRadius.circular(AppRadius.small),
              onTap: onForgotPressed,
              child: Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppSpacing.sm,
                  vertical: AppSpacing.sm,
                ),
                child: FittedBox(
                  fit: BoxFit.scaleDown,
                  child: GradientText(
                    l10n.authLoginForgotPassword,
                    gradient: AppGradients.bluePurple,
                    style: textTheme.bodyMedium?.copyWith(
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ),
              ),
            ),
          ),
        ),
      ],
    );
  }
}

/// "Don't have an account?" prompt with a tappable sign-up link.
///
/// Mirrors the forgot-password link styling (gradient text on an [InkWell])
/// so the whole prompt opens the sign-up screen.
class _SignUpPrompt extends StatelessWidget {
  const _SignUpPrompt({required this.onSignUpPressed});

  final VoidCallback? onSignUpPressed;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final textTheme = Theme.of(context).textTheme;
    return Center(
      child: InkWell(
        borderRadius: BorderRadius.circular(AppRadius.small),
        onTap: onSignUpPressed,
        child: Padding(
          padding: const EdgeInsets.symmetric(
            horizontal: AppSpacing.sm,
            vertical: AppSpacing.sm,
          ),
          child: FittedBox(
            fit: BoxFit.scaleDown,
            child: Row(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.baseline,
              textBaseline: TextBaseline.alphabetic,
              children: <Widget>[
                Text(
                  l10n.authLoginDontHaveAccount,
                  style: textTheme.bodyMedium?.copyWith(
                    color: Colors.white.withValues(alpha: 0.7),
                  ),
                ),
                const SizedBox(width: AppSpacing.xs),
                GradientText(
                  l10n.authLoginSignUp,
                  gradient: AppGradients.bluePurple,
                  style: textTheme.bodyMedium?.copyWith(
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

/// Pill-style toggle used for the remember-me switch.
///
/// Matches the approved mockup: a light-grey track with a white thumb that
/// slides to the right and tints blue when enabled.
class _RememberSwitch extends StatelessWidget {
  const _RememberSwitch({required this.value, required this.onChanged});

  final bool value;
  final ValueChanged<bool> onChanged;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      behavior: HitTestBehavior.opaque,
      onTap: () => onChanged(!value),
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 180),
        width: 44,
        height: 22,
        padding: const EdgeInsets.all(2),
        decoration: BoxDecoration(
          color: value ? AppColors.blue600 : AppColors.rememberTrack,
          borderRadius: BorderRadius.circular(11),
        ),
        child: AnimatedAlign(
          duration: const Duration(milliseconds: 180),
          alignment: value ? Alignment.centerRight : Alignment.centerLeft,
          child: Container(
            width: 18,
            height: 18,
            decoration: const BoxDecoration(
              color: AppColors.surface,
              shape: BoxShape.circle,
            ),
          ),
        ),
      ),
    );
  }
}

/// "—— OR ——" separator between the primary action and secondary options.
class _OrDivider extends StatelessWidget {
  const _OrDivider({required this.label});

  final String label;

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;
    return Row(
      children: <Widget>[
        const Expanded(
          child: Divider(color: AppColors.whiteBorder, thickness: 1),
        ),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: AppSpacing.md),
          child: Text(
            label,
            style: textTheme.bodyMedium?.copyWith(
              color: Colors.white.withValues(alpha: 0.7),
              fontWeight: FontWeight.w600,
            ),
          ),
        ),
        const Expanded(
          child: Divider(color: AppColors.whiteBorder, thickness: 1),
        ),
      ],
    );
  }
}
