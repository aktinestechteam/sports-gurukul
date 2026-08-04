import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';

/// An animated pill tab bar.
///
/// The active tab slides and fades its gradient pill in via [AnimatedContainer]
/// while the label color transitions with [AnimatedDefaultTextStyle] - pure
/// implicit animation, no manual controllers.
class AnimatedTabBar extends StatelessWidget {
  const AnimatedTabBar({
    required this.labels,
    required this.index,
    required this.onChanged,
    super.key,
    this.activeGradient = AppGradients.primary,
    this.height = 48,
  });

  /// Tab labels, drawn left to right.
  final List<String> labels;

  /// Currently selected index (validated against [labels].length).
  final int index;

  /// Called with the newly selected index.
  final ValueChanged<int> onChanged;

  /// Gradient pill painted behind the active tab.
  final Gradient activeGradient;

  /// Bar height in logical pixels.
  final double height;

  @override
  Widget build(BuildContext context) {
    final safeIndex = index.clamp(0, labels.length - 1);
    return Container(
      height: height,
      padding: const EdgeInsets.all(4),
      decoration: BoxDecoration(
        color: AppColors.glassFill,
        borderRadius: BorderRadius.circular(AppRadius.xlarge),
      ),
      child: Row(
        children: List<Widget>.generate(labels.length, (i) {
          final selected = i == safeIndex;
          return Expanded(
            child: GestureDetector(
              onTap: () => onChanged(i),
              behavior: HitTestBehavior.opaque,
              child: AnimatedContainer(
                duration: const Duration(milliseconds: 260),
                curve: Curves.easeOutCubic,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(AppRadius.large),
                  gradient: selected ? activeGradient : null,
                  boxShadow: selected
                      ? const <BoxShadow>[
                          BoxShadow(
                            color: Color(0x44006DFF),
                            blurRadius: 12,
                            offset: Offset(0, 4),
                          ),
                        ]
                      : const <BoxShadow>[],
                ),
                alignment: Alignment.center,
                child: AnimatedDefaultTextStyle(
                  duration: const Duration(milliseconds: 200),
                  style: Theme.of(context).textTheme.labelLarge!.copyWith(
                    color: selected ? AppColors.surface : AppColors.grey300,
                    fontWeight: selected ? FontWeight.w700 : FontWeight.w600,
                  ),
                  child: Text(
                    labels[i],
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                ),
              ),
            ),
          );
        }),
      ),
    );
  }
}
