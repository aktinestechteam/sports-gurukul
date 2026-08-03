/// Convenience extensions on [Iterable].
extension IterableX<T> on Iterable<T> {
  /// The first element matching [test], or `null` when none matches.
  T? firstWhereOrNull(bool Function(T element) test) {
    for (final element in this) {
      if (test(element)) {
        return element;
      }
    }
    return null;
  }

  /// The last element matching [test], or `null` when none matches.
  T? lastWhereOrNull(bool Function(T element) test) {
    T? match;
    for (final element in this) {
      if (test(element)) {
        match = element;
      }
    }
    return match;
  }

  /// The element at [index], or `null` when [index] is out of range.
  T? elementAtOrNull(int index) {
    if (index < 0) {
      return null;
    }
    var current = 0;
    for (final element in this) {
      if (current == index) {
        return element;
      }
      current++;
    }
    return null;
  }

  /// The number of elements matching [test].
  int countWhere(bool Function(T element) test) {
    var count = 0;
    for (final element in this) {
      if (test(element)) {
        count++;
      }
    }
    return count;
  }

  /// Maps each element with its zero-based index.
  Iterable<R> mapIndexed<R>(R Function(int index, T element) transform) sync* {
    var index = 0;
    for (final element in this) {
      yield transform(index, element);
      index++;
    }
  }

  /// Groups elements by the key produced by [keyOf].
  Map<K, List<T>> groupBy<K>(K Function(T element) keyOf) {
    final groups = <K, List<T>>{};
    for (final element in this) {
      groups.putIfAbsent(keyOf(element), () => <T>[]).add(element);
    }
    return groups;
  }

  /// Sums the values produced by [selector] across all elements.
  num sumBy(num Function(T element) selector) {
    num sum = 0;
    for (final element in this) {
      sum += selector(element);
    }
    return sum;
  }
}

/// Convenience extensions on iterables of nullable values.
extension IterableNullableX<T> on Iterable<T?> {
  /// Returns the non-null elements in iteration order.
  Iterable<T> whereNotNull() => whereType<T>();
}
