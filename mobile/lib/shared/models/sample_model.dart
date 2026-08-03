import 'package:freezed_annotation/freezed_annotation.dart';

part 'sample_model.freezed.dart';
part 'sample_model.g.dart';

/// Code-generation pipeline validation sample.
///
/// Confirms that build_runner + freezed + json_serializable generate
/// correctly (immutability, `==`/`hashCode`, `copyWith`, JSON round-trip).
/// This is NOT a domain or DTO model: it is removed when real models arrive
/// in P004.
@freezed
abstract class SampleModel with _$SampleModel {
  const factory SampleModel({
    required String id,
    required String name,
    int? count,
  }) = _SampleModel;

  factory SampleModel.fromJson(Map<String, dynamic> json) =>
      _$SampleModelFromJson(json);
}
