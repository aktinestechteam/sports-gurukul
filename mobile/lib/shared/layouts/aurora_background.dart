import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';

/// A vibrant animated-ready canvas for dashboard/home screens.
///
/// Paints a deep indigo gradient base and three soft radial "aurora" blobs
/// behind [child]. Blobs are static [DecoratedBox]es (no continuous repaint)
/// and sit on their own layer, so the effect stays cheap on device GPUs.
/// Place glass surfaces on top so the [BackdropFilter] has colour to blur.
class AuroraBackground extends StatelessWidget {
  const AuroraBackground({required this.child, super.key});

  /// Foreground content rendered above the aurora.
  final Widget child;

  @override
  Widget build(BuildContext context) {
    return DecoratedBox(
      decoration: const BoxDecoration(gradient: AppGradients.background),
      child: Stack(
        fit: StackFit.expand,
        children: <Widget>[
          const _Blob(
            alignment: Alignment(-0.9, -0.8),
            size: 300,
            gradient: AppGradients.blobViolet,
          ),
          const _Blob(
            alignment: Alignment(1.1, -0.35),
            size: 260,
            gradient: AppGradients.blobCyan,
          ),
          const _Blob(
            alignment: Alignment(-0.4, 1.1),
            size: 320,
            gradient: AppGradients.blobPink,
          ),
          RepaintBoundary(child: child),
        ],
      ),
    );
  }
}

/// A single radial gradient blob pinned to [alignment].
class _Blob extends StatelessWidget {
  const _Blob({
    required this.alignment,
    required this.size,
    required this.gradient,
  });

  final Alignment alignment;
  final double size;
  final RadialGradient gradient;

  @override
  Widget build(BuildContext context) {
    return Align(
      alignment: alignment,
      child: IgnorePointer(
        child: Container(
          width: size,
          height: size,
          decoration: BoxDecoration(shape: BoxShape.circle, gradient: gradient),
        ),
      ),
    );
  }
}
