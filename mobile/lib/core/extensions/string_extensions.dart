import 'package:sports_gurukul/core/utils/parser.dart';

/// Convenience extensions on [String].
extension StringX on String {
  /// Whether the string is empty or contains only whitespace.
  bool get isBlank => trim().isEmpty;

  /// Returns `null` when this string is blank, otherwise itself.
  String? get nullIfBlank => isBlank ? null : this;

  /// Parses this string as an [int], or `null` when it is not numeric.
  int? toIntOrNull() => Parser.intOrNull(this);

  /// Parses this string as a [double], or `null` when it is not numeric.
  double? toDoubleOrNull() => Parser.doubleOrNull(this);

  /// Returns this string with the first character uppercased.
  String capitalize() {
    if (isEmpty) {
      return this;
    }
    return this[0].toUpperCase() + substring(1);
  }

  /// Capitalizes the first character of every whitespace-separated word.
  String titleCase() => split(' ').map(_capitalizeWord).join(' ');

  /// Removes every whitespace character from this string.
  String removeAllWhitespace() => replaceAll(RegExp(r'\s'), '');

  /// Truncates this string to [maxLength] characters, appending
  /// [ellipsis] when truncated.
  String truncate(int maxLength, {String ellipsis = '...'}) {
    if (length <= maxLength) {
      return this;
    }
    if (maxLength <= 0) {
      return ellipsis;
    }
    return substring(0, maxLength).trimRight() + ellipsis;
  }

  String _capitalizeWord(String word) =>
      word.isEmpty ? word : word.capitalize();
}
