import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/app/theme/app_shadow.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/validators/email_validator.dart';
import 'package:sports_gurukul/core/validators/required_validator.dart';
import 'package:sports_gurukul/core/validators/validator.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/authentication/presentation/widgets/auth_messages.dart';
import 'package:sports_gurukul/features/authentication/presentation/widgets/auth_scaffold.dart';
import 'package:sports_gurukul/features/authentication/presentation/widgets/auth_text_field.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/buttons/gradient_button.dart';
import 'package:sports_gurukul/shared/text/gradient_text.dart';

/// Requests a password reset email for the entered account.
class ForgotPasswordPage extends ConsumerStatefulWidget {
  const ForgotPasswordPage({super.key});

  @override
  ConsumerState<ForgotPasswordPage> createState() => _ForgotPasswordPageState();
}

class _ForgotPasswordPageState extends ConsumerState<ForgotPasswordPage> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _emailController;
  bool _submitting = false;

  @override
  void initState() {
    super.initState();
    _emailController = TextEditingController();
  }

  @override
  void dispose() {
    _emailController.dispose();
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
        .forgotPassword(_emailController.text.trim());
    if (!mounted) {
      return;
    }
    setState(() => _submitting = false);
    result.when(
      onSuccess: () {
        final l10n = AppLocalizations.of(context);
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(l10n.authForgotEmailSent)),
        );
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

    return AuthScaffold(
      title: l10n.authForgotTitle,
      subtitle: l10n.authForgotSubtitle,
      child: Form(
        key: _formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: <Widget>[
            AuthTextField(
              controller: _emailController,
              label: l10n.authLoginEmailLabel,
              icon: Icons.mail_outline,
              keyboardType: TextInputType.emailAddress,
              textInputAction: TextInputAction.done,
              autofillHints: const <String>[AutofillHints.email],
              validator: (value) => AuthMessages.validation(
                context,
                const CompositeValidator<String>(<Validator<String>>[
                  RequiredValidator(),
                  EmailValidator(),
                ]).validate(value),
              ),
              onFieldSubmitted: (_) => _submitting ? null : _submit(),
            ),
            const SizedBox(height: AppSpacing.xxxl),
            GradientButton(
              label: l10n.authForgotSubmit,
              gradient: AppGradients.bluePurpleHorizontal,
              onPressed: _submitting ? null : _submit,
              loading: _submitting,
              shadows: AppShadow.glowPrimary,
              borderRadius: BorderRadius.circular(AppRadius.input),
            ),
            const SizedBox(height: AppSpacing.sm),
            Center(
              child: InkWell(
                borderRadius: BorderRadius.circular(AppRadius.small),
                onTap: _submitting ? null : () => context.go(RoutePaths.login),
                child: Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppSpacing.sm,
                    vertical: AppSpacing.sm,
                  ),
                  child: FittedBox(
                    fit: BoxFit.scaleDown,
                    child: GradientText(
                      l10n.authForgotBackToLogin,
                      gradient: AppGradients.bluePurple,
                      style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
