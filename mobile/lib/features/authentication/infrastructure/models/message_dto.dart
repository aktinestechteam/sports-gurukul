import 'package:freezed_annotation/freezed_annotation.dart';

part 'message_dto.freezed.dart';
part 'message_dto.g.dart';

/// Response payload for message-only auth endpoints
/// (`MessageResponse` contract): send/verify email, forgot/reset password.
@freezed
abstract class MessageDto with _$MessageDto {
  const factory MessageDto({
    required String message,
  }) = _MessageDto;

  factory MessageDto.fromJson(Map<String, dynamic> json) =>
      _$MessageDtoFromJson(json);
}
