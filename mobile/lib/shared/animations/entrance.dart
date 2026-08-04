import 'dart:async';

import 'package:flutter/material.dart';

/// Staggered entrance transition built on Flutter's implicit animated widgets.
///
/// Fades, slides and scales [child] in once on mount. [delay] staggers
/// sibling entrances so a screen feels choreographed rather than static.
/// Widgets stay fully interactive during the transition and repaint cheaply
/// (transform + opacity only).
class Entrance extends StatefulWidget {
  const Entrance({
    required this.child,
    super.key,
    this.delay = Duration.zero,
    this.duration = const Duration(milliseconds: 600),
    this.curve = Curves.easeOutCubic,
    this.offset = const Offset(0, 0.12),
    this.scaleFrom = 0.97,
  });

  /// The content to reveal.
  final Widget child;

  /// Delay before the entrance starts (used for staggering siblings).
  final Duration delay;

  /// How long the entrance takes.
  final Duration duration;

  /// Motion curve for the entrance.
  final Curve curve;

  /// Start offset, as a fraction of the child's own size.
  final Offset offset;

  /// Start scale (just under 1 for a gentle grow-in).
  final double scaleFrom;

  @override
  State<Entrance> createState() => _EntranceState();
}

class _EntranceState extends State<Entrance> {
  bool _visible = false;
  Timer? _timer;

  @override
  void initState() {
    super.initState();
    if (widget.delay == Duration.zero) {
      WidgetsBinding.instance.addPostFrameCallback((_) => _reveal());
    } else {
      _timer = Timer(widget.delay, _reveal);
    }
  }

  void _reveal() {
    if (!mounted) return;
    setState(() => _visible = true);
  }

  @override
  void dispose() {
    _timer?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AnimatedOpacity(
      opacity: _visible ? 1 : 0,
      duration: widget.duration,
      curve: widget.curve,
      child: AnimatedSlide(
        offset: _visible ? Offset.zero : widget.offset,
        duration: widget.duration,
        curve: widget.curve,
        child: AnimatedScale(
          scale: _visible ? 1 : widget.scaleFrom,
          duration: widget.duration,
          curve: widget.curve,
          child: widget.child,
        ),
      ),
    );
  }
}
