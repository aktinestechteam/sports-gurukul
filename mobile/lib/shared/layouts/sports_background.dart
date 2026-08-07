import 'package:flutter/material.dart';

class SportsBackground extends StatelessWidget {
  const SportsBackground({
    super.key,
    this.child,
  });

  static String getAsset(BuildContext context) {
    final width = MediaQuery.of(context).size.width;

    if (width >= 1200) {
      return 'assets/images/login_desktop.png';
    } else if (width >= 700) {
      return 'assets/images/login_tablet.png';
    } else {
      return 'assets/images/login_mobile.png';
    }
  }

  final Widget? child;

  @override
  Widget build(BuildContext context) {
    return Stack(
      fit: StackFit.expand,
      children: [
        // Full-screen background
        Positioned.fill(
          child: Image.asset(
            getAsset(context),
            fit: BoxFit.cover,
            filterQuality: FilterQuality.high,
            isAntiAlias: true,
            gaplessPlayback: true,
          ),
        ),

        // Light overlay for readability
        Positioned.fill(
          child: Container(
            color: Colors.black.withValues(alpha: 0.18),
          ),
        ),

        // Very subtle vignette
        Positioned.fill(
          child: IgnorePointer(
            child: DecoratedBox(
              decoration: BoxDecoration(
                gradient: RadialGradient(
                  radius: 1.25,
                  colors: [
                    Colors.transparent,
                    Colors.black.withValues(alpha: 0.18),
                  ],
                  stops: const [
                    0.65,
                    1.0,
                  ],
                ),
              ),
            ),
          ),
        ),

        ?child,
      ],
    );
  }
}
