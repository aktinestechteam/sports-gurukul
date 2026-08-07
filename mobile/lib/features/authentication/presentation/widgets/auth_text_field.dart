import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

/// Text field used across authentication forms.
///
/// Password fields get a visibility toggle; all fields accept a validator
/// that returns localized error text via the `Form`.
///
/// The [InputDecoration] is declared explicitly (not left to the theme) so
/// the floating label behaves exactly like Material 3 while never being
/// clipped: labels float on focus and `clipBehavior` is `none` so the label
/// can never be cut off by the field's own hard-edge clip while animating.
///
/// Every colour defaults to the light-on-dark palette used by the login
/// screen so every auth form matches over the dark glass card. The optional
/// `*Color` parameters override any of those defaults.
class AuthTextField extends StatefulWidget {
  const AuthTextField({
    required this.controller,
    required this.label,
    this.icon,
    this.obscureText = false,
    this.keyboardType,
    this.textInputAction,
    this.autofillHints,
    this.validator,
    this.onFieldSubmitted,
    this.iconColor,
    this.labelColor,
    this.floatingLabelColor,
    this.textColor,
    this.fillColor,
    this.enabledBorderColor,
    this.focusedBorderColor,
    this.errorColor,
    this.floatingLabelBehavior = FloatingLabelBehavior.never,
    super.key,
  });

  final TextEditingController controller;
  final String label;
  final IconData? icon;
  final bool obscureText;
  final TextInputType? keyboardType;
  final TextInputAction? textInputAction;
  final Iterable<String>? autofillHints;
  final String? Function(String?)? validator;
  final void Function(String)? onFieldSubmitted;

  /// Prefix/suffix icon colour (e.g. white on the dark login card).
  final Color? iconColor;

  /// Resting label colour (the grey "placeholder" look while unfocused).
  final Color? labelColor;

  /// Floated label colour (e.g. brand blue once focused or always-filled).
  final Color? floatingLabelColor;

  /// Input text and caret colour.
  final Color? textColor;

  /// Background fill; `Colors.transparent` gives the reference glass look.
  final Color? fillColor;

  /// Unfocused border colour.
  final Color? enabledBorderColor;

  /// Focused border colour.
  final Color? focusedBorderColor;

  /// Error state border colour.
  final Color? errorColor;

  /// Controls when the floating label floats above the field; auth forms use
  /// [FloatingLabelBehavior.never] so labels stay centred inside the field,
  /// exactly like the approved login mockup.
  final FloatingLabelBehavior floatingLabelBehavior;

  @override
  State<AuthTextField> createState() => _AuthTextFieldState();
}

class _AuthTextFieldState extends State<AuthTextField> {
  late bool _obscure;

  @override
  void initState() {
    super.initState();
    _obscure = widget.obscureText;
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final canToggle = widget.obscureText;
    // Defaults match the light-on-dark palette used across the auth forms.
    final textColor = widget.textColor ?? AppColors.surface;
    final fillColor = widget.fillColor ?? AppColors.loginFieldFill;
    final iconColor = widget.iconColor ?? AppColors.surface;
    final enabledBorderColor =
        widget.enabledBorderColor ?? AppColors.grey400.withValues(alpha: 0.7);
    final focusedBorderColor = widget.focusedBorderColor ?? AppColors.blue500;
    final errorColor = widget.errorColor ?? AppColors.danger;

    return TextFormField(
      controller: widget.controller,
      obscureText: _obscure,
      keyboardType: widget.keyboardType,
      textInputAction: widget.textInputAction,
      autofillHints: widget.autofillHints?.toList(),
      validator: widget.validator,
      onFieldSubmitted: widget.onFieldSubmitted,
      autocorrect: !widget.obscureText,
      enableSuggestions: !widget.obscureText,
      clipBehavior: Clip.none,
      cursorColor: textColor,
      style: TextStyle(color: textColor),
      decoration: InputDecoration(
        labelText: widget.label,
        floatingLabelBehavior: widget.floatingLabelBehavior,
        prefixIcon: widget.icon == null
            ? null
            : Icon(widget.icon, color: iconColor),
        suffixIcon: canToggle
            ? IconButton(
                icon: Icon(
                  _obscure ? Icons.visibility_off : Icons.visibility,
                  color: iconColor,
                ),
                tooltip: _obscure
                    ? l10n.authShowPassword
                    : l10n.authHidePassword,
                onPressed: () => setState(() => _obscure = !_obscure),
              )
            : null,
        filled: true,
        fillColor: fillColor,
        labelStyle: TextStyle(
          color: widget.labelColor ?? AppColors.loginSubtitle,
        ),
        floatingLabelStyle: TextStyle(
          color: widget.floatingLabelColor ?? AppColors.blue500,
          fontWeight: FontWeight.w600,
        ),
        contentPadding: const EdgeInsets.symmetric(
          horizontal: AppSpacing.lg,
          vertical: AppSpacing.lg,
        ),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppRadius.input),
          borderSide: BorderSide(color: enabledBorderColor),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppRadius.input),
          borderSide: BorderSide(color: enabledBorderColor),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppRadius.input),
          borderSide: BorderSide(color: focusedBorderColor, width: 2),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppRadius.input),
          borderSide: BorderSide(color: errorColor),
        ),
        focusedErrorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppRadius.input),
          borderSide: BorderSide(color: errorColor, width: 2),
        ),
      ),
    );
  }
}
