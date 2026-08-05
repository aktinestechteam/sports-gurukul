// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'profile_photo_response_dto.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$ProfilePhotoResponseDto {

 String get fileId; String get url; String get fileName; int get fileSize; String get contentType; String get uploadedAt;
/// Create a copy of ProfilePhotoResponseDto
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$ProfilePhotoResponseDtoCopyWith<ProfilePhotoResponseDto> get copyWith => _$ProfilePhotoResponseDtoCopyWithImpl<ProfilePhotoResponseDto>(this as ProfilePhotoResponseDto, _$identity);

  /// Serializes this ProfilePhotoResponseDto to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is ProfilePhotoResponseDto&&(identical(other.fileId, fileId) || other.fileId == fileId)&&(identical(other.url, url) || other.url == url)&&(identical(other.fileName, fileName) || other.fileName == fileName)&&(identical(other.fileSize, fileSize) || other.fileSize == fileSize)&&(identical(other.contentType, contentType) || other.contentType == contentType)&&(identical(other.uploadedAt, uploadedAt) || other.uploadedAt == uploadedAt));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,fileId,url,fileName,fileSize,contentType,uploadedAt);

@override
String toString() {
  return 'ProfilePhotoResponseDto(fileId: $fileId, url: $url, fileName: $fileName, fileSize: $fileSize, contentType: $contentType, uploadedAt: $uploadedAt)';
}


}

/// @nodoc
abstract mixin class $ProfilePhotoResponseDtoCopyWith<$Res>  {
  factory $ProfilePhotoResponseDtoCopyWith(ProfilePhotoResponseDto value, $Res Function(ProfilePhotoResponseDto) _then) = _$ProfilePhotoResponseDtoCopyWithImpl;
@useResult
$Res call({
 String fileId, String url, String fileName, int fileSize, String contentType, String uploadedAt
});




}
/// @nodoc
class _$ProfilePhotoResponseDtoCopyWithImpl<$Res>
    implements $ProfilePhotoResponseDtoCopyWith<$Res> {
  _$ProfilePhotoResponseDtoCopyWithImpl(this._self, this._then);

  final ProfilePhotoResponseDto _self;
  final $Res Function(ProfilePhotoResponseDto) _then;

/// Create a copy of ProfilePhotoResponseDto
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? fileId = null,Object? url = null,Object? fileName = null,Object? fileSize = null,Object? contentType = null,Object? uploadedAt = null,}) {
  return _then(_self.copyWith(
fileId: null == fileId ? _self.fileId : fileId // ignore: cast_nullable_to_non_nullable
as String,url: null == url ? _self.url : url // ignore: cast_nullable_to_non_nullable
as String,fileName: null == fileName ? _self.fileName : fileName // ignore: cast_nullable_to_non_nullable
as String,fileSize: null == fileSize ? _self.fileSize : fileSize // ignore: cast_nullable_to_non_nullable
as int,contentType: null == contentType ? _self.contentType : contentType // ignore: cast_nullable_to_non_nullable
as String,uploadedAt: null == uploadedAt ? _self.uploadedAt : uploadedAt // ignore: cast_nullable_to_non_nullable
as String,
  ));
}

}


/// Adds pattern-matching-related methods to [ProfilePhotoResponseDto].
extension ProfilePhotoResponseDtoPatterns on ProfilePhotoResponseDto {
/// A variant of `map` that fallback to returning `orElse`.
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case final Subclass value:
///     return ...;
///   case _:
///     return orElse();
/// }
/// ```

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _ProfilePhotoResponseDto value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _ProfilePhotoResponseDto() when $default != null:
return $default(_that);case _:
  return orElse();

}
}
/// A `switch`-like method, using callbacks.
///
/// Callbacks receives the raw object, upcasted.
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case final Subclass value:
///     return ...;
///   case final Subclass2 value:
///     return ...;
/// }
/// ```

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _ProfilePhotoResponseDto value)  $default,){
final _that = this;
switch (_that) {
case _ProfilePhotoResponseDto():
return $default(_that);case _:
  throw StateError('Unexpected subclass');

}
}
/// A variant of `map` that fallback to returning `null`.
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case final Subclass value:
///     return ...;
///   case _:
///     return null;
/// }
/// ```

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _ProfilePhotoResponseDto value)?  $default,){
final _that = this;
switch (_that) {
case _ProfilePhotoResponseDto() when $default != null:
return $default(_that);case _:
  return null;

}
}
/// A variant of `when` that fallback to an `orElse` callback.
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case Subclass(:final field):
///     return ...;
///   case _:
///     return orElse();
/// }
/// ```

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String fileId,  String url,  String fileName,  int fileSize,  String contentType,  String uploadedAt)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _ProfilePhotoResponseDto() when $default != null:
return $default(_that.fileId,_that.url,_that.fileName,_that.fileSize,_that.contentType,_that.uploadedAt);case _:
  return orElse();

}
}
/// A `switch`-like method, using callbacks.
///
/// As opposed to `map`, this offers destructuring.
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case Subclass(:final field):
///     return ...;
///   case Subclass2(:final field2):
///     return ...;
/// }
/// ```

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String fileId,  String url,  String fileName,  int fileSize,  String contentType,  String uploadedAt)  $default,) {final _that = this;
switch (_that) {
case _ProfilePhotoResponseDto():
return $default(_that.fileId,_that.url,_that.fileName,_that.fileSize,_that.contentType,_that.uploadedAt);case _:
  throw StateError('Unexpected subclass');

}
}
/// A variant of `when` that fallback to returning `null`
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case Subclass(:final field):
///     return ...;
///   case _:
///     return null;
/// }
/// ```

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String fileId,  String url,  String fileName,  int fileSize,  String contentType,  String uploadedAt)?  $default,) {final _that = this;
switch (_that) {
case _ProfilePhotoResponseDto() when $default != null:
return $default(_that.fileId,_that.url,_that.fileName,_that.fileSize,_that.contentType,_that.uploadedAt);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _ProfilePhotoResponseDto implements ProfilePhotoResponseDto {
  const _ProfilePhotoResponseDto({required this.fileId, required this.url, required this.fileName, required this.fileSize, required this.contentType, required this.uploadedAt});
  factory _ProfilePhotoResponseDto.fromJson(Map<String, dynamic> json) => _$ProfilePhotoResponseDtoFromJson(json);

@override final  String fileId;
@override final  String url;
@override final  String fileName;
@override final  int fileSize;
@override final  String contentType;
@override final  String uploadedAt;

/// Create a copy of ProfilePhotoResponseDto
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$ProfilePhotoResponseDtoCopyWith<_ProfilePhotoResponseDto> get copyWith => __$ProfilePhotoResponseDtoCopyWithImpl<_ProfilePhotoResponseDto>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$ProfilePhotoResponseDtoToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _ProfilePhotoResponseDto&&(identical(other.fileId, fileId) || other.fileId == fileId)&&(identical(other.url, url) || other.url == url)&&(identical(other.fileName, fileName) || other.fileName == fileName)&&(identical(other.fileSize, fileSize) || other.fileSize == fileSize)&&(identical(other.contentType, contentType) || other.contentType == contentType)&&(identical(other.uploadedAt, uploadedAt) || other.uploadedAt == uploadedAt));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,fileId,url,fileName,fileSize,contentType,uploadedAt);

@override
String toString() {
  return 'ProfilePhotoResponseDto(fileId: $fileId, url: $url, fileName: $fileName, fileSize: $fileSize, contentType: $contentType, uploadedAt: $uploadedAt)';
}


}

/// @nodoc
abstract mixin class _$ProfilePhotoResponseDtoCopyWith<$Res> implements $ProfilePhotoResponseDtoCopyWith<$Res> {
  factory _$ProfilePhotoResponseDtoCopyWith(_ProfilePhotoResponseDto value, $Res Function(_ProfilePhotoResponseDto) _then) = __$ProfilePhotoResponseDtoCopyWithImpl;
@override @useResult
$Res call({
 String fileId, String url, String fileName, int fileSize, String contentType, String uploadedAt
});




}
/// @nodoc
class __$ProfilePhotoResponseDtoCopyWithImpl<$Res>
    implements _$ProfilePhotoResponseDtoCopyWith<$Res> {
  __$ProfilePhotoResponseDtoCopyWithImpl(this._self, this._then);

  final _ProfilePhotoResponseDto _self;
  final $Res Function(_ProfilePhotoResponseDto) _then;

/// Create a copy of ProfilePhotoResponseDto
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? fileId = null,Object? url = null,Object? fileName = null,Object? fileSize = null,Object? contentType = null,Object? uploadedAt = null,}) {
  return _then(_ProfilePhotoResponseDto(
fileId: null == fileId ? _self.fileId : fileId // ignore: cast_nullable_to_non_nullable
as String,url: null == url ? _self.url : url // ignore: cast_nullable_to_non_nullable
as String,fileName: null == fileName ? _self.fileName : fileName // ignore: cast_nullable_to_non_nullable
as String,fileSize: null == fileSize ? _self.fileSize : fileSize // ignore: cast_nullable_to_non_nullable
as int,contentType: null == contentType ? _self.contentType : contentType // ignore: cast_nullable_to_non_nullable
as String,uploadedAt: null == uploadedAt ? _self.uploadedAt : uploadedAt // ignore: cast_nullable_to_non_nullable
as String,
  ));
}


}

// dart format on
