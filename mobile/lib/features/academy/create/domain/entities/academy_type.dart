/// How an academy structures its sports programs.
///
/// Collected by the create-academy wizard and persisted on the backend
/// academy record via `AcademyType`.
enum AcademyType {
  /// The academy focuses on a single sport.
  singleSport('SingleSport'),

  /// The academy offers several sports.
  multiSport('MultiSport');

  const AcademyType(this.wireValue);

  /// The value sent to the backend `CreateAcademyRequest.AcademyType`.
  final String wireValue;
}
