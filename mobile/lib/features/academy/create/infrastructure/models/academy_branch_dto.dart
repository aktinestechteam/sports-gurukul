import 'package:freezed_annotation/freezed_annotation.dart';

part 'academy_branch_dto.freezed.dart';
part 'academy_branch_dto.g.dart';

/// DTO matching the backend `BranchDto` schema returned inside
/// `AcademyDto.Branches`.
@freezed
abstract class AcademyBranchDto with _$AcademyBranchDto {
  const factory AcademyBranchDto({
    String? id,
    String? academyId,
    String? branchName,
    String? address,
    String? country,
    String? state,
    String? city,
    String? district,
    String? postalCode,
    String? createdAt,
    String? updatedAt,
  }) = _AcademyBranchDto;

  factory AcademyBranchDto.fromJson(Map<String, dynamic> json) =>
      _$AcademyBranchDtoFromJson(json);
}
