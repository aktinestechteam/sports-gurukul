import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/app/theme/app_shadow.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/validators/password_validator.dart';
import 'package:sports_gurukul/core/validators/required_validator.dart';
import 'package:sports_gurukul/core/validators/validation_error.dart';
import 'package:sports_gurukul/core/validators/validator.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/authentication/presentation/widgets/auth_messages.dart';
import 'package:sports_gurukul/features/authentication/presentation/widgets/auth_scaffold.dart';
import 'package:sports_gurukul/features/authentication/presentation/widgets/auth_text_field.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/buttons/gradient_button.dart';
import 'package:sports_gurukul/shared/text/gradient_text.dart';

/// Sets a new password using the token from the reset email.
class ResetPasswordPage extends ConsumerStatefulWidget {
  const ResetPasswordPage({super.key, this.token});

  /// The reset token, normally supplied via the deep link
  /// `/reset-password?token=...`.
  final String? token;

  @override
  ConsumerState<ResetPasswordPage> createState() => _ResetPasswordPageState();
}

class _ResetPasswordPageState extends ConsumerState<ResetPasswordPage> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _newPasswordController;
  late final TextEditingController _confirmController;
  bool _submitting = false;

  static const Validator<String> _newPasswordValidator = PasswordValidator(
    requireUppercase: true,
    requireLowercase: true,
    requireDigit: true,
    requireSpecialCharacter: true,
  );

  @override
  void initState() {
    super.initState();
    _newPasswordController = TextEditingController();
    _confirmController = TextEditingController();
  }

  @override
  void dispose() {
    _newPasswordController.dispose();
    _confirmController.dispose();
    super.dispose();
  }

  ValidationError? _validateNewPassword(String? value) {
    final required = const RequiredValidator<String>().validate(value);
    return required ?? _newPasswordValidator.validate(value);
  }

  ValidationError? _validateConfirmation(String? value) {
    final required = const RequiredValidator<String>().validate(value);
    if (required != null) {
      return required;
    }
    if (value != _newPasswordController.text) {
      return const ValidationError('validation.password.mismatch');
    }
    return null;
  }

  Future<void> _submit() async {
    FocusScope.of(context).unfocus();
    if (!_formKey.currentState!.validate()) {
      return;
    }
    final token = widget.token;
    if (token == null || token.isEmpty) {
      _showMessage(AppLocalizations.of(context).authResetMissingToken);
      return;
    }
    setState(() => _submitting = true);
    final result = await ref
        .read(authControllerProvider.notifier)
        .resetPassword(
          token: token,
          newPassword: _newPasswordController.text,
          confirmNewPassword: _confirmController.text,
        );
    if (!mounted) {
      return;
    }
    setState(() => _submitting = false);
    result.when(
      onSuccess: () {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(AppLocalizations.of(context).authResetSuccess),
          ),
        );
        context.go(RoutePaths.login);
      },
      onFailure: _showFailure,
    );
  }

  void _showFailure(BaseFailure failure) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(AuthMessages.apiFailure(context, failure))),
    );
  }

  void _showMessage(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(message)),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);

    return AuthScaffold(
      title: l10n.authResetTitle,
      subtitle: l10n.authResetSubtitle,
      child: Form(
        key: _formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: <Widget>[
            AuthTextField(
              controller: _newPasswordController,
              label: l10n.authResetNewPasswordLabel,
              icon: Icons.lock_outline,
              obscureText: true,
              textInputAction: TextInputAction.next,
              autofillHints: const <String>[AutofillHints.newPassword],
              validator: (value) =>
                  AuthMessages.validation(context, _validateNewPassword(value)),
            ),
            const SizedBox(height: AppSpacing.lg),
            AuthTextField(
              controller: _confirmController,
              label: l10n.authResetConfirmPasswordLabel,
              icon: Icons.lock_outline,
              obscureText: true,
              textInputAction: TextInputAction.done,
              autofillHints: const <String>[AutofillHints.newPassword],
              validator: (value) => AuthMessages.validation(
                context,
                _validateConfirmation(value),
              ),
              onFieldSubmitted: (_) => _submitting ? null : _submit(),
            ),
            const SizedBox(height: AppSpacing.xxxl),
            GradientButton(
              label: l10n.authResetSubmit,
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
