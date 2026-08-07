/// Convenience extensions on [List].
extension ListX<T> on List<T> {
  /// Replaces the first element matching [test] with [element], or appends
  /// [element] when no element matches. Mutates and returns this list.
  List<T> replaceOrAppend(T element, {required bool Function(T) test}) {
    final index = indexWhere(test);
    if (index == -1) {
      add(element);
    } else {
      this[index] = element;
    }
    return this;
  }

  /// Whether any element matches [test].
  bool containsWhere(bool Function(T element) test) => indexWhere(test) != -1;

  /// The index of the first element matching [test], or `null` when none
  /// matches.
  int? indexWhereOrNull(bool Function(T element) test) {
    final index = indexWhere(test);
    return index == -1 ? null : index;
  }

  /// A new list containing each distinct element (by equality) in order.
  List<T> unique() {
    final seen = <T>{};
    final result = <T>[];
    for (final element in this) {
      if (seen.add(element)) {
        result.add(element);
      }
    }
    return result;
  }

  /// A new list sorted by [compare], leaving this list unchanged.
  List<T> sortedCopy([int Function(T a, T b)? compare]) =>
      [...this]..sort(compare);
}
