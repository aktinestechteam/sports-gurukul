import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';

/// Dark dropdown used across the profile forms.
///
/// Matches the profile text field styling (rounded corners, translucent dark
/// fill, light-on-dark text) so every control on the dark glass forms looks
/// consistent. The popup menu inherits the app theme; items render on the
/// current surface colors.
class ProfileDropdownField<T> extends StatelessWidget {
  const ProfileDropdownField({
    required this.label,
    required this.value,
    required this.items,
    required this.onChanged,
    super.key,
    this.icon,
    this.enabled = true,
    this.validator,
  });

  final String label;
  final T? value;
  final List<DropdownMenuItem<T>> items;
  final ValueChanged<T?> onChanged;
  final IconData? icon;
  final bool enabled;
  final String? Function(T?)? validator;

  @override
  Widget build(BuildContext context) {
    return DropdownButtonFormField<T>(
      initialValue: value,
      validator: validator,
      items: items,
      onChanged: enabled ? onChanged : null,
      dropdownColor: AppColors.surfaceDark,
      iconEnabledColor: AppColors.surface,
      iconDisabledColor: AppColors.grey500,
      style: const TextStyle(color: AppColors.surface),
      decoration: InputDecoration(
        labelText: label,
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
