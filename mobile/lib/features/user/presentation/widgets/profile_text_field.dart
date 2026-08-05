import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';

/// Dark text field used across the profile forms.
///
/// Mirrors the authentication fields: rounded corners, translucent dark fill,
/// leading icon and light-on-dark text. Unlike the auth fields, the label
/// floats above the field ([FloatingLabelBehavior.auto]) so long data-entry
/// forms stay legible; pass a [validator] to get inline localized errors via
/// the surrounding `Form`.
class ProfileTextField extends StatelessWidget {
  const ProfileTextField({
    required this.controller,
    required this.label,
    super.key,
    this.icon,
    this.validator,
    this.keyboardType,
    this.textInputAction,
    this.maxLines = 1,
    this.prefixText,
    this.onFieldSubmitted,
  });

  final TextEditingController controller;
  final String label;
  final IconData? icon;
  final String? Function(String?)? validator;
  final TextInputType? keyboardType;
  final TextInputAction? textInputAction;
  final int maxLines;
  final String? prefixText;
  final void Function(String)? onFieldSubmitted;

  @override
  Widget build(BuildContext context) {
    return TextFormField(
      controller: controller,
      validator: validator,
      keyboardType: keyboardType,
      textInputAction: textInputAction,
      maxLines: maxLines,
      onFieldSubmitted: onFieldSubmitted,
      style: const TextStyle(color: AppColors.surface),
      cursorColor: AppColors.blue500,
      decoration: InputDecoration(
        labelText: label,
        alignLabelWithHint: maxLines > 1,
        prefixText: prefixText,
        prefixIcon: icon == null ? null : Icon(icon, color: AppColors.surface),
        filled: true,
        fillColor: AppColors.loginFieldFill,
        labelStyle: const TextStyle(color: AppColors.loginSubtitle),
        floatingLabelStyle: const TextStyle(
          color: AppColors.blue500,
          fontWeight: FontWeight.w600,
        ),
        contentPadding: const EdgeInsets.symmetric(
          horizontal: AppSpacing.lg,
          vertical: AppSpacing.lg,
        ),
        border: _border(AppColors.grey400.withValues(alpha: 0.7)),
        enabledBorder: _border(AppColors.grey400.withValues(alpha: 0.7)),
        focusedBorder: _border(AppColors.blue500, width: 2),
        errorBorder: _border(AppColors.danger),
        focusedErrorBorder: _border(AppColors.danger, width: 2),
      ),
    );
  }

  static OutlineInputBorder _border(Color color, {double width = 1}) {
    return OutlineInputBorder(
      borderRadius: BorderRadius.circular(AppRadius.input),
      borderSide: BorderSide(color: color, width: width),
    );
  }
}
