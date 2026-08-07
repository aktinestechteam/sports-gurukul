import 'dart:ui';

import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/shared/animations/spring_press.dart';

/// Frosted circular home button that hands the user back to the dashboard,
/// shared by the create- and edit-academy wizards.
class AcademyHomeButton extends StatelessWidget {
  const AcademyHomeButton({
    required this.onPressed,
    required this.tooltip,
    super.key,
  });

  final VoidCallback onPressed;
  final String tooltip;

  @override
  Widget build(BuildContext context) {
    return Tooltip(
      message: tooltip,
      child: SpringPress(
        onPressed: onPressed,
        scaleDown: 0.9,
        child: ClipOval(
          child: BackdropFilter(
            filter: ImageFilter.blur(sigmaX: 10, sigmaY: 10),
            child: Container(
              width: 44,
              height: 44,
              decoration: const BoxDecoration(
                shape: BoxShape.circle,
                color: AppColors.glassFill,
                border: Border.fromBorderSide(
                  BorderSide(color: AppColors.glassBorderLo),
                ),
              ),
              child: const Icon(
                Icons.home_rounded,
                color: AppColors.surface,
                size: 20,
              ),
            ),
          ),
        ),
      ),
    );
  }
}
