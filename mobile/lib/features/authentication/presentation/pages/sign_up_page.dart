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
import 'package:sports_gurukul/core/validators/password_validator.dart';
import 'package:sports_gurukul/core/validators/phone_validator.dart';
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

/// Creates a new Sports Gurukul account.
///
/// Shares the [AuthScaffold] shell used by the other authentication screens
/// and, on success, lets the auth-aware redirect land on the dashboard.
class SignUpPage extends ConsumerStatefulWidget {
  const SignUpPage({super.key});

  @override
  ConsumerState<SignUpPage> createState() => _SignUpPageState();
}

class _SignUpPageState extends ConsumerState<SignUpPage> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _nameController;
  late final TextEditingController _emailController;
  late final TextEditingController _phoneController;
  late final TextEditingController _passwordController;
  late final TextEditingController _confirmController;
  bool _submitting = false;

  static const Validator<String> _passwordValidator = PasswordValidator(
    requireUppercase: true,
    requireLowercase: true,
    requireDigit: true,
    requireSpecialCharacter: true,
  );

  @override
  void initState() {
    super.initState();
    _nameController = TextEditingController();
    _emailController = TextEditingController();
    _phoneController = TextEditingController();
    _passwordController = TextEditingController();
    _confirmController = TextEditingController();
  }

  @override
  void dispose() {
    _nameController.dispose();
    _emailController.dispose();
    _phoneController.dispose();
    _passwordController.dispose();
    _confirmController.dispose();
    super.dispose();
  }

  ValidationError? _validatePassword(String? value) {
    final required = const RequiredValidator<String>().validate(value);
    return required ?? _passwordValidator.validate(value);
  }

  ValidationError? _validateConfirmation(String? value) {
    final required = const RequiredValidator<String>().validate(value);
    if (required != null) {
      return required;
    }
    if (value != _passwordController.text) {
      return const ValidationError('validation.password.mismatch');
    }
    return null;
  }

  Future<void> _submit() async {
    FocusScope.of(context).unfocus();
    if (!_formKey.currentState!.validate()) {
      return;
    }
    setState(() => _submitting = true);
    final phoneNumber = _phoneController.text.trim();
    final result = await ref
        .read(authControllerProvider.notifier)
        .register(
          fullName: _nameController.text.trim(),
          email: _emailController.text.trim(),
          password: _passwordController.text,
          confirmPassword: _confirmController.text,
          phoneNumber: phoneNumber.isEmpty ? null : phoneNumber,
        );
    if (!mounted) {
      return;
    }
    setState(() => _submitting = false);
    result.when(
      onSuccess: (_) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(AppLocalizations.of(context).authSignUpSuccess),
          ),
        );
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

    return AuthScaffold(
      title: l10n.authSignUpTitle,
      subtitle: l10n.authSignUpSubtitle,
      child: Form(
        key: _formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: <Widget>[
            AuthTextField(
              controller: _nameController,
              label: l10n.authSignUpNameLabel,
              icon: Icons.person_outline,
              textInputAction: TextInputAction.next,
              autofillHints: const <String>[AutofillHints.name],
              validator: (value) => AuthMessages.validation(
                context,
                const RequiredValidator<String>().validate(value),
              ),
            ),
            const SizedBox(height: AppSpacing.lg),
            AuthTextField(
              controller: _emailController,
              label: l10n.authLoginEmailLabel,
              icon: Icons.mail_outline,
              keyboardType: TextInputType.emailAddress,
              textInputAction: TextInputAction.next,
              autofillHints: const <String>[AutofillHints.email],
              validator: (value) => AuthMessages.validation(
                context,
                const CompositeValidator<String>(<Validator<String>>[
                  RequiredValidator(),
                  EmailValidator(),
                ]).validate(value),
              ),
            ),
            const SizedBox(height: AppSpacing.lg),
            AuthTextField(
              controller: _phoneController,
              label: l10n.authSignUpPhoneLabel,
              icon: Icons.phone_outlined,
              keyboardType: TextInputType.phone,
              textInputAction: TextInputAction.next,
              autofillHints: const <String>[AutofillHints.telephoneNumber],
              validator: (value) => AuthMessages.validation(
                context,
                const PhoneValidator().validate(value),
              ),
            ),
            const SizedBox(height: AppSpacing.lg),
            AuthTextField(
              controller: _passwordController,
              label: l10n.authLoginPasswordLabel,
              icon: Icons.lock_outline,
              obscureText: true,
              textInputAction: TextInputAction.next,
              autofillHints: const <String>[AutofillHints.newPassword],
              validator: (value) => AuthMessages.validation(
                context,
                _validatePassword(value),
              ),
            ),
            const SizedBox(height: AppSpacing.lg),
            AuthTextField(
              controller: _confirmController,
              label: l10n.authSignUpConfirmPasswordLabel,
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
              label: l10n.authSignUpSubmit,
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
                    child: Row(
                      mainAxisSize: MainAxisSize.min,
                      children: <Widget>[
                        Text(
                          l10n.authSignUpAlreadyHaveAccount,
                          style: Theme.of(context).textTheme.bodyMedium
                              ?.copyWith(
                                color: Colors.white.withValues(alpha: 0.7),
                              ),
                        ),
                        const SizedBox(width: AppSpacing.xs),
                        GradientText(
                          l10n.authSignUpSignIn,
                          gradient: AppGradients.bluePurple,
                          style: Theme.of(context).textTheme.bodyMedium
                              ?.copyWith(
                                fontWeight: FontWeight.w700,
                              ),
                        ),
                      ],
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
