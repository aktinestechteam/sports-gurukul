import 'package:flutter/material.dart';

/// Centralized shadow tokens for the Sports Gurukul design system.
///
/// Shadows must remain subtle; heavy shadows are an anti-pattern per the
/// mobile design system.
abstract final class AppShadow {
  static const List<BoxShadow> small = <BoxShadow>[
    BoxShadow(
      color: Color(0x14000000),
      blurRadius: 4,
      offset: Offset(0, 1),
    ),
  ];

  static const List<BoxShadow> medium = <BoxShadow>[
    BoxShadow(
      color: Color(0x1F000000),
      blurRadius: 8,
      offset: Offset(0, 2),
    ),
  ];

  static const List<BoxShadow> large = <BoxShadow>[
    BoxShadow(
      color: Color(0x29000000),
      blurRadius: 16,
      offset: Offset(0, 4),
    ),
  ];

  /// Layered glass shadow: a tight dark core under a wide, soft halo.
  ///
  /// Kept airy (large blur, low opacity) so the frosted card floats without
  /// looking heavy against the photo or aurora background.
  static const List<BoxShadow> glass = <BoxShadow>[
    BoxShadow(
      color: Color(0x1F000000),
      blurRadius: 24,
      offset: Offset(0, 8),
    ),
    BoxShadow(
      color: Color(0x0D000000),
      blurRadius: 48,
      offset: Offset(0, 16),
    ),
  ];

  /// Colored glow tinted with the primary blue.
  static const List<BoxShadow> glowPrimary = <BoxShadow>[
    BoxShadow(
      color: Color(0x591299FF),
      blurRadius: 14,
      offset: Offset(0, 6),
    ),
    BoxShadow(
      color: Color(0x261299FF),
      blurRadius: 34,
      offset: Offset(0, 14),
    ),
  ];

  /// Colored glow tinted with the aurora violet/blue.
  static const List<BoxShadow> glowAurora = <BoxShadow>[
    BoxShadow(
      color: Color(0x598B5CF6),
      blurRadius: 14,
      offset: Offset(0, 6),
    ),
    BoxShadow(
      color: Color(0x26006DFF),
      blurRadius: 34,
      offset: Offset(0, 14),
    ),
  ];

  /// Colored glow tinted with the sunset orange/pink.
  static const List<BoxShadow> glowSunset = <BoxShadow>[
    BoxShadow(
      color: Color(0x59FF8A00),
      blurRadius: 14,
      offset: Offset(0, 6),
    ),
    BoxShadow(
      color: Color(0x26EC4899),
      blurRadius: 34,
      offset: Offset(0, 14),
    ),
  ];

  /// Colored glow tinted with emerald.
  static const List<BoxShadow> glowEmerald = <BoxShadow>[
    BoxShadow(
      color: Color(0x5910B981),
      blurRadius: 14,
      offset: Offset(0, 6),
    ),
    BoxShadow(
      color: Color(0x2616A34A),
      blurRadius: 34,
      offset: Offset(0, 14),
    ),
  ];
}
