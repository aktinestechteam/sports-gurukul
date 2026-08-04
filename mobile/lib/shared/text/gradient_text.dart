import 'package:flutter/material.dart';

/// Renders [text] filled with a [Gradient] using a [ShaderMask].
///
/// The text keeps its own [style] (size, weight, height); only the fill
/// colour is replaced by the gradient. Used for brand headings and links on
/// the authentication screens.
class GradientText extends StatelessWidget {
  const GradientText(
    this.text, {
    required this.gradient,
    this.style,
    this.textAlign,
    super.key,
  });

  /// The text to paint.
  final String text;

  /// Gradient painted through the glyphs.
  final Gradient gradient;

  /// Typography applied on top of the gradient fill.
  final TextStyle? style;

  /// Alignment of the text block.
  final TextAlign? textAlign;

  @override
  Widget build(BuildContext context) {
    return ShaderMask(
      blendMode: BlendMode.srcIn,
      shaderCallback: gradient.createShader,
      child: Text(
        text,
        textAlign: textAlign,
        style: (style ?? const TextStyle()).copyWith(color: Colors.white),
      ),
    );
  }
}
