/// Curated sports catalog for the create-academy wizard.
///
/// The backend seeds sports with deterministic identifiers but exposes no
/// lookup endpoint, so the wizard offers this static list. Selections are
/// stored client-side only; the create-academy API has no sport-assignment
/// step in its request contract yet.
abstract final class AcademySports {
  /// The selectable sports, alphabetically ordered.
  static const List<String> catalog = <String>[
    'Athletics',
    'Badminton',
    'Basketball',
    'Boxing',
    'Cricket',
    'Football',
    'Hockey',
    'Kabaddi',
    'Swimming',
    'Table Tennis',
    'Tennis',
    'Volleyball',
  ];
}
