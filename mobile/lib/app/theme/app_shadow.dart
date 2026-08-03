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
}
