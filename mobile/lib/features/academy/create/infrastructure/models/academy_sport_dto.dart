import 'package:freezed_annotation/freezed_annotation.dart';

part 'academy_sport_dto.freezed.dart';
part 'academy_sport_dto.g.dart';

/// DTO matching the backend `AcademySportDto` schema returned inside
/// `AcademyDto.Sports`.
@freezed
abstract class AcademySportDto with _$AcademySportDto {
  const factory AcademySportDto({
    String? id,
    String? sportId,
    String? name,
    String? code,
    @Default(false) bool isPrimarySport,
  }) = _AcademySportDto;

  factory AcademySportDto.fromJson(Map<String, dynamic> json) =>
      _$AcademySportDtoFromJson(json);
}
