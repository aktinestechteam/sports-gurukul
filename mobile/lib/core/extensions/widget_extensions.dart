import 'package:flutter/material.dart';

/// Convenience extensions for composing widgets with common wrappers.
extension WidgetX on Widget {
  /// Wraps this widget in [Padding] with [value] on every side.
  Widget paddingAll(double value) =>
      Padding(padding: EdgeInsets.all(value), child: this);

  /// Wraps this widget in [Padding] with [value] on the horizontal axes.
  Widget paddingHorizontal(double value) => Padding(
    padding: EdgeInsets.symmetric(horizontal: value),
    child: this,
  );

  /// Wraps this widget in [Padding] with [value] on the vertical axes.
  Widget paddingVertical(double value) => Padding(
    padding: EdgeInsets.symmetric(vertical: value),
    child: this,
  );

  /// Wraps this widget in [Padding] with per-axis [horizontal]/[vertical].
  Widget paddingSymmetric({double horizontal = 0, double vertical = 0}) =>
      Padding(
        padding: EdgeInsets.symmetric(
          horizontal: horizontal,
          vertical: vertical,
        ),
        child: this,
      );

  /// Wraps this widget in [Padding] with the given [padding].
  Widget padding(EdgeInsets padding) => Padding(padding: padding, child: this);

  /// Wraps this widget in a [Container] with [value] on every side.
  Widget marginAll(double value) =>
      Container(margin: EdgeInsets.all(value), child: this);

  /// Wraps this widget in a [Container] with per-axis [horizontal]/[vertical].
  Widget marginSymmetric({double horizontal = 0, double vertical = 0}) =>
      Container(
        margin: EdgeInsets.symmetric(
          horizontal: horizontal,
          vertical: vertical,
        ),
        child: this,
      );

  /// Centers this widget inside a [Center].
  Widget center() => Center(child: this);

  /// Aligns this widget with [alignment].
  Widget align(AlignmentGeometry alignment) =>
      Align(alignment: alignment, child: this);

  /// Makes this widget expand to fill available space in a flex parent.
  Widget expanded({int flex = 1}) => Expanded(flex: flex, child: this);

  /// Wraps this widget in a [SliverToBoxAdapter].
  Widget sliverToBoxAdapter() => SliverToBoxAdapter(child: this);

  /// Shows or hides this widget using [Visibility].
  Widget visible({required bool visible, Widget? replacement}) => Visibility(
    visible: visible,
    replacement: replacement ?? const SizedBox.shrink(),
    child: this,
  );

  /// Attaches a [GestureDetector] that invokes [onTap].
  Widget onTap(VoidCallback onTap) =>
      GestureDetector(onTap: onTap, child: this);

  /// Wraps this widget in a [Tooltip] with [message].
  Widget withTooltip(String message) => Tooltip(message: message, child: this);
}
