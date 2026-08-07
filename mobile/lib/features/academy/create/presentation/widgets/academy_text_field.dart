import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';

/// Dark text field for the create-academy wizard.
///
/// Mirrors the visual language of the profile text fields but is
/// controller-less: it seeds from [initialValue] and reports changes through
/// [onChanged], so the wizard can keep all state inside the controller
/// instead of managing `TextEditingController` lifetimes.
class AcademyTextField extends StatelessWidget {
  const AcademyTextField({
    required this.label,
    required this.initialValue,
    required this.onChanged,
    super.key,
    this.icon,
    this.validator,
    this.keyboardType,
    this.textInputAction,
    this.maxLines = 1,
    this.prefixText,
  });

  final String label;
  final String initialValue;
  final ValueChanged<String> onChanged;
  final IconData? icon;
  final String? Function(String?)? validator;
  final TextInputType? keyboardType;
  final TextInputAction? textInputAction;
  final int maxLines;
  final String? prefixText;

  @override
  Widget build(BuildContext context) {
    return TextFormField(
      initialValue: initialValue,
      onChanged: onChanged,
      validator: validator,
      keyboardType: keyboardType,
      textInputAction: textInputAction,
      maxLines: maxLines,
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
