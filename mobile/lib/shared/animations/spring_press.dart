import 'package:flutter/material.dart';

/// Wraps [child] with a springy press interaction.
///
/// Pressing scales the child down; releasing snaps it back with a soft
/// overshoot ([Curves.easeOutBack]) for a tactile, high-grade feel.
/// Taps are routed through [onPressed], which also disables the interaction
/// when null. [enabled] mirrors `Material` disabled behaviour.
class SpringPress extends StatefulWidget {
  const SpringPress({
    required this.child,
    this.onPressed,
    super.key,
    this.enabled = true,
    this.scaleDown = 0.96,
    this.duration = const Duration(milliseconds: 260),
  });

  /// The surface to make pressable.
  final Widget child;

  /// Called when the press is released inside the surface.
  final VoidCallback? onPressed;

  /// Whether the interaction is active. `false` disables taps and animation.
  final bool enabled;

  /// Pressed scale factor (slightly below 1 for a tactile squeeze).
  final double scaleDown;

  /// Duration of the release (spring back) animation.
  final Duration duration;

  @override
  State<SpringPress> createState() => _SpringPressState();
}

class _SpringPressState extends State<SpringPress> {
  bool _pressed = false;

  bool get _active => widget.enabled && widget.onPressed != null;

  void _handleTapDown(TapDownDetails details) {
    if (!_active) return;
    setState(() => _pressed = true);
  }

  void _handleTapUp(TapUpDetails details) {
    if (!_active) return;
    setState(() => _pressed = false);
  }

  void _handleTapCancel() {
    if (!_active) return;
    setState(() => _pressed = false);
  }

  void _handleTap() {
    if (!_active) return;
    widget.onPressed!();
  }

  @override
  Widget build(BuildContext context) {
    return Semantics(
      button: true,
      enabled: _active,
      child: GestureDetector(
        behavior: HitTestBehavior.opaque,
        onTapDown: _handleTapDown,
        onTapUp: _handleTapUp,
        onTapCancel: _handleTapCancel,
        onTap: _handleTap,
        child: AnimatedScale(
          scale: _pressed ? widget.scaleDown : 1,
          duration: widget.duration,
          curve: _pressed ? Curves.easeOut : Curves.easeOutBack,
          child: widget.child,
        ),
      ),
    );
  }
}
