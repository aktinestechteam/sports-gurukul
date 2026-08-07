// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'user_profile_dto.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$UserProfileDto {

 String get id; String get userId; String get fullName; String get email; String get createdAt; String? get phoneNumber; String? get dateOfBirth;@JsonKey(unknownEnumValue: GenderDto.preferNotToSay) GenderDto get gender; String? get bio; String? get profileImageUrl; String? get coverImageUrl; String? get height; String? get weight; String? get preferredSport; String? get experienceLevel;@JsonKey(unknownEnumValue: UserStatusDto.active) UserStatusDto get status; bool get isEmailVerified; String? get updatedAt; int get profileCompletionPercentage; List<AddressDto> get addresses; ContactDto? get contactInformation; UserPreferenceDto? get preferences; List<String> get roles; bool get hasProfile;
/// Create a copy of UserProfileDto
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$UserProfileDtoCopyWith<UserProfileDto> get copyWith => _$UserProfileDtoCopyWithImpl<UserProfileDto>(this as UserProfileDto, _$identity);

  /// Serializes this UserProfileDto to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is UserProfileDto&&(identical(other.id, id) || other.id == id)&&(identical(other.userId, userId) || other.userId == userId)&&(identical(other.fullName, fullName) || other.fullName == fullName)&&(identical(other.email, email) || other.email == email)&&(identical(other.createdAt, createdAt) || other.createdAt == createdAt)&&(identical(other.phoneNumber, phoneNumber) || other.phoneNumber == phoneNumber)&&(identical(other.dateOfBirth, dateOfBirth) || other.dateOfBirth == dateOfBirth)&&(identical(other.gender, gender) || other.gender == gender)&&(identical(other.bio, bio) || other.bio == bio)&&(identical(other.profileImageUrl, profileImageUrl) || other.profileImageUrl == profileImageUrl)&&(identical(other.coverImageUrl, coverImageUrl) || other.coverImageUrl == coverImageUrl)&&(identical(other.height, height) || other.height == height)&&(identical(other.weight, weight) || other.weight == weight)&&(identical(other.preferredSport, preferredSport) || other.preferredSport == preferredSport)&&(identical(other.experienceLevel, experienceLevel) || other.experienceLevel == experienceLevel)&&(identical(other.status, status) || other.status == status)&&(identical(other.isEmailVerified, isEmailVerified) || other.isEmailVerified == isEmailVerified)&&(identical(other.updatedAt, updatedAt) || other.updatedAt == updatedAt)&&(identical(other.profileCompletionPercentage, profileCompletionPercentage) || other.profileCompletionPercentage == profileCompletionPercentage)&&const DeepCollectionEquality().equals(other.addresses, addresses)&&(identical(other.contactInformation, contactInformation) || other.contactInformation == contactInformation)&&(identical(other.preferences, preferences) || other.preferences == preferences)&&const DeepCollectionEquality().equals(other.roles, roles)&&(identical(other.hasProfile, hasProfile) || other.hasProfile == hasProfile));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hashAll([runtimeType,id,userId,fullName,email,createdAt,phoneNumber,dateOfBirth,gender,bio,profileImageUrl,coverImageUrl,height,weight,preferredSport,experienceLevel,status,isEmailVerified,updatedAt,profileCompletionPercentage,const DeepCollectionEquality().hash(addresses),contactInformation,preferences,const DeepCollectionEquality().hash(roles),hasProfile]);

@override
String toString() {
  return 'UserProfileDto(id: $id, userId: $userId, fullName: $fullName, email: $email, createdAt: $createdAt, phoneNumber: $phoneNumber, dateOfBirth: $dateOfBirth, gender: $gender, bio: $bio, profileImageUrl: $profileImageUrl, coverImageUrl: $coverImageUrl, height: $height, weight: $weight, preferredSport: $preferredSport, experienceLevel: $experienceLevel, status: $status, isEmailVerified: $isEmailVerified, updatedAt: $updatedAt, profileCompletionPercentage: $profileCompletionPercentage, addresses: $addresses, contactInformation: $contactInformation, preferences: $preferences, roles: $roles, hasProfile: $hasProfile)';
}


}

/// @nodoc
abstract mixin class $UserProfileDtoCopyWith<$Res>  {
  factory $UserProfileDtoCopyWith(UserProfileDto value, $Res Function(UserProfileDto) _then) = _$UserProfileDtoCopyWithImpl;
@useResult
$Res call({
 String id, String userId, String fullName, String email, String createdAt, String? phoneNumber, String? dateOfBirth,@JsonKey(unknownEnumValue: GenderDto.preferNotToSay) GenderDto gender, String? bio, String? profileImageUrl, String? coverImageUrl, String? height, String? weight, String? preferredSport, String? experienceLevel,@JsonKey(unknownEnumValue: UserStatusDto.active) UserStatusDto status, bool isEmailVerified, String? updatedAt, int profileCompletionPercentage, List<AddressDto> addresses, ContactDto? contactInformation, UserPreferenceDto? preferences, List<String> roles, bool hasProfile
});


$ContactDtoCopyWith<$Res>? get contactInformation;$UserPreferenceDtoCopyWith<$Res>? get preferences;

}
/// @nodoc
class _$UserProfileDtoCopyWithImpl<$Res>
    implements $UserProfileDtoCopyWith<$Res> {
  _$UserProfileDtoCopyWithImpl(this._self, this._then);

  final UserProfileDto _self;
  final $Res Function(UserProfileDto) _then;

/// Create a copy of UserProfileDto
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = null,Object? userId = null,Object? fullName = null,Object? email = null,Object? createdAt = null,Object? phoneNumber = freezed,Object? dateOfBirth = freezed,Object? gender = null,Object? bio = freezed,Object? profileImageUrl = freezed,Object? coverImageUrl = freezed,Object? height = freezed,Object? weight = freezed,Object? preferredSport = freezed,Object? experienceLevel = freezed,Object? status = null,Object? isEmailVerified = null,Object? updatedAt = freezed,Object? profileCompletionPercentage = null,Object? addresses = null,Object? contactInformation = freezed,Object? preferences = freezed,Object? roles = null,Object? hasProfile = null,}) {
  return _then(_self.copyWith(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,userId: null == userId ? _self.userId : userId // ignore: cast_nullable_to_non_nullable
as String,fullName: null == fullName ? _self.fullName : fullName // ignore: cast_nullable_to_non_nullable
as String,email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,createdAt: null == createdAt ? _self.createdAt : createdAt // ignore: cast_nullable_to_non_nullable
as String,phoneNumber: freezed == phoneNumber ? _self.phoneNumber : phoneNumber // ignore: cast_nullable_to_non_nullable
as String?,dateOfBirth: freezed == dateOfBirth ? _self.dateOfBirth : dateOfBirth // ignore: cast_nullable_to_non_nullable
as String?,gender: null == gender ? _self.gender : gender // ignore: cast_nullable_to_non_nullable
as GenderDto,bio: freezed == bio ? _self.bio : bio // ignore: cast_nullable_to_non_nullable
as String?,profileImageUrl: freezed == profileImageUrl ? _self.profileImageUrl : profileImageUrl // ignore: cast_nullable_to_non_nullable
as String?,coverImageUrl: freezed == coverImageUrl ? _self.coverImageUrl : coverImageUrl // ignore: cast_nullable_to_non_nullable
as String?,height: freezed == height ? _self.height : height // ignore: cast_nullable_to_non_nullable
as String?,weight: freezed == weight ? _self.weight : weight // ignore: cast_nullable_to_non_nullable
as String?,preferredSport: freezed == preferredSport ? _self.preferredSport : preferredSport // ignore: cast_nullable_to_non_nullable
as String?,experienceLevel: freezed == experienceLevel ? _self.experienceLevel : experienceLevel // ignore: cast_nullable_to_non_nullable
as String?,status: null == status ? _self.status : status // ignore: cast_nullable_to_non_nullable
as UserStatusDto,isEmailVerified: null == isEmailVerified ? _self.isEmailVerified : isEmailVerified // ignore: cast_nullable_to_non_nullable
as bool,updatedAt: freezed == updatedAt ? _self.updatedAt : updatedAt // ignore: cast_nullable_to_non_nullable
as String?,profileCompletionPercentage: null == profileCompletionPercentage ? _self.profileCompletionPercentage : profileCompletionPercentage // ignore: cast_nullable_to_non_nullable
as int,addresses: null == addresses ? _self.addresses : addresses // ignore: cast_nullable_to_non_nullable
as List<AddressDto>,contactInformation: freezed == contactInformation ? _self.contactInformation : contactInformation // ignore: cast_nullable_to_non_nullable
as ContactDto?,preferences: freezed == preferences ? _self.preferences : preferences // ignore: cast_nullable_to_non_nullable
as UserPreferenceDto?,roles: null == roles ? _self.roles : roles // ignore: cast_nullable_to_non_nullable
as List<String>,hasProfile: null == hasProfile ? _self.hasProfile : hasProfile // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}
/// Create a copy of UserProfileDto
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ContactDtoCopyWith<$Res>? get contactInformation {
    if (_self.contactInformation == null) {
    return null;
  }

  return $ContactDtoCopyWith<$Res>(_self.contactInformation!, (value) {
    return _then(_self.copyWith(contactInformation: value));
  });
}/// Create a copy of UserProfileDto
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$UserPreferenceDtoCopyWith<$Res>? get preferences {
    if (_self.preferences == null) {
    return null;
  }

  return $UserPreferenceDtoCopyWith<$Res>(_self.preferences!, (value) {
    return _then(_self.copyWith(preferences: value));
  });
}
}


/// Adds pattern-matching-related methods to [UserProfileDto].
extension UserProfileDtoPatterns on UserProfileDto {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _UserProfileDto value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _UserProfileDto() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _UserProfileDto value)  $default,){
final _that = this;
switch (_that) {
case _UserProfileDto():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _UserProfileDto value)?  $default,){
final _that = this;
switch (_that) {
case _UserProfileDto() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String id,  String userId,  String fullName,  String email,  String createdAt,  String? phoneNumber,  String? dateOfBirth, @JsonKey(unknownEnumValue: GenderDto.preferNotToSay)  GenderDto gender,  String? bio,  String? profileImageUrl,  String? coverImageUrl,  String? height,  String? weight,  String? preferredSport,  String? experienceLevel, @JsonKey(unknownEnumValue: UserStatusDto.active)  UserStatusDto status,  bool isEmailVerified,  String? updatedAt,  int profileCompletionPercentage,  List<AddressDto> addresses,  ContactDto? contactInformation,  UserPreferenceDto? preferences,  List<String> roles,  bool hasProfile)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _UserProfileDto() when $default != null:
return $default(_that.id,_that.userId,_that.fullName,_that.email,_that.createdAt,_that.phoneNumber,_that.dateOfBirth,_that.gender,_that.bio,_that.profileImageUrl,_that.coverImageUrl,_that.height,_that.weight,_that.preferredSport,_that.experienceLevel,_that.status,_that.isEmailVerified,_that.updatedAt,_that.profileCompletionPercentage,_that.addresses,_that.contactInformation,_that.preferences,_that.roles,_that.hasProfile);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String id,  String userId,  String fullName,  String email,  String createdAt,  String? phoneNumber,  String? dateOfBirth, @JsonKey(unknownEnumValue: GenderDto.preferNotToSay)  GenderDto gender,  String? bio,  String? profileImageUrl,  String? coverImageUrl,  String? height,  String? weight,  String? preferredSport,  String? experienceLevel, @JsonKey(unknownEnumValue: UserStatusDto.active)  UserStatusDto status,  bool isEmailVerified,  String? updatedAt,  int profileCompletionPercentage,  List<AddressDto> addresses,  ContactDto? contactInformation,  UserPreferenceDto? preferences,  List<String> roles,  bool hasProfile)  $default,) {final _that = this;
switch (_that) {
case _UserProfileDto():
return $default(_that.id,_that.userId,_that.fullName,_that.email,_that.createdAt,_that.phoneNumber,_that.dateOfBirth,_that.gender,_that.bio,_that.profileImageUrl,_that.coverImageUrl,_that.height,_that.weight,_that.preferredSport,_that.experienceLevel,_that.status,_that.isEmailVerified,_that.updatedAt,_that.profileCompletionPercentage,_that.addresses,_that.contactInformation,_that.preferences,_that.roles,_that.hasProfile);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String id,  String userId,  String fullName,  String email,  String createdAt,  String? phoneNumber,  String? dateOfBirth, @JsonKey(unknownEnumValue: GenderDto.preferNotToSay)  GenderDto gender,  String? bio,  String? profileImageUrl,  String? coverImageUrl,  String? height,  String? weight,  String? preferredSport,  String? experienceLevel, @JsonKey(unknownEnumValue: UserStatusDto.active)  UserStatusDto status,  bool isEmailVerified,  String? updatedAt,  int profileCompletionPercentage,  List<AddressDto> addresses,  ContactDto? contactInformation,  UserPreferenceDto? preferences,  List<String> roles,  bool hasProfile)?  $default,) {final _that = this;
switch (_that) {
case _UserProfileDto() when $default != null:
return $default(_that.id,_that.userId,_that.fullName,_that.email,_that.createdAt,_that.phoneNumber,_that.dateOfBirth,_that.gender,_that.bio,_that.profileImageUrl,_that.coverImageUrl,_that.height,_that.weight,_that.preferredSport,_that.experienceLevel,_that.status,_that.isEmailVerified,_that.updatedAt,_that.profileCompletionPercentage,_that.addresses,_that.contactInformation,_that.preferences,_that.roles,_that.hasProfile);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _UserProfileDto implements UserProfileDto {
  const _UserProfileDto({required this.id, required this.userId, required this.fullName, required this.email, required this.createdAt, this.phoneNumber, this.dateOfBirth, @JsonKey(unknownEnumValue: GenderDto.preferNotToSay) this.gender = GenderDto.preferNotToSay, this.bio, this.profileImageUrl, this.coverImageUrl, this.height, this.weight, this.preferredSport, this.experienceLevel, @JsonKey(unknownEnumValue: UserStatusDto.active) this.status = UserStatusDto.active, this.isEmailVerified = false, this.updatedAt, this.profileCompletionPercentage = 0, final  List<AddressDto> addresses = const [], this.contactInformation, this.preferences, final  List<String> roles = const [], this.hasProfile = true}): _addresses = addresses,_roles = roles;
  factory _UserProfileDto.fromJson(Map<String, dynamic> json) => _$UserProfileDtoFromJson(json);

@override final  String id;
@override final  String userId;
@override final  String fullName;
@override final  String email;
@override final  String createdAt;
@override final  String? phoneNumber;
@override final  String? dateOfBirth;
@override@JsonKey(unknownEnumValue: GenderDto.preferNotToSay) final  GenderDto gender;
@override final  String? bio;
@override final  String? profileImageUrl;
@override final  String? coverImageUrl;
@override final  String? height;
@override final  String? weight;
@override final  String? preferredSport;
@override final  String? experienceLevel;
@override@JsonKey(unknownEnumValue: UserStatusDto.active) final  UserStatusDto status;
@override@JsonKey() final  bool isEmailVerified;
@override final  String? updatedAt;
@override@JsonKey() final  int profileCompletionPercentage;
 final  List<AddressDto> _addresses;
@override@JsonKey() List<AddressDto> get addresses {
  if (_addresses is EqualUnmodifiableListView) return _addresses;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_addresses);
}

@override final  ContactDto? contactInformation;
@override final  UserPreferenceDto? preferences;
 final  List<String> _roles;
@override@JsonKey() List<String> get roles {
  if (_roles is EqualUnmodifiableListView) return _roles;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_roles);
}

@override@JsonKey() final  bool hasProfile;

/// Create a copy of UserProfileDto
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$UserProfileDtoCopyWith<_UserProfileDto> get copyWith => __$UserProfileDtoCopyWithImpl<_UserProfileDto>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$UserProfileDtoToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _UserProfileDto&&(identical(other.id, id) || other.id == id)&&(identical(other.userId, userId) || other.userId == userId)&&(identical(other.fullName, fullName) || other.fullName == fullName)&&(identical(other.email, email) || other.email == email)&&(identical(other.createdAt, createdAt) || other.createdAt == createdAt)&&(identical(other.phoneNumber, phoneNumber) || other.phoneNumber == phoneNumber)&&(identical(other.dateOfBirth, dateOfBirth) || other.dateOfBirth == dateOfBirth)&&(identical(other.gender, gender) || other.gender == gender)&&(identical(other.bio, bio) || other.bio == bio)&&(identical(other.profileImageUrl, profileImageUrl) || other.profileImageUrl == profileImageUrl)&&(identical(other.coverImageUrl, coverImageUrl) || other.coverImageUrl == coverImageUrl)&&(identical(other.height, height) || other.height == height)&&(identical(other.weight, weight) || other.weight == weight)&&(identical(other.preferredSport, preferredSport) || other.preferredSport == preferredSport)&&(identical(other.experienceLevel, experienceLevel) || other.experienceLevel == experienceLevel)&&(identical(other.status, status) || other.status == status)&&(identical(other.isEmailVerified, isEmailVerified) || other.isEmailVerified == isEmailVerified)&&(identical(other.updatedAt, updatedAt) || other.updatedAt == updatedAt)&&(identical(other.profileCompletionPercentage, profileCompletionPercentage) || other.profileCompletionPercentage == profileCompletionPercentage)&&const DeepCollectionEquality().equals(other._addresses, _addresses)&&(identical(other.contactInformation, contactInformation) || other.contactInformation == contactInformation)&&(identical(other.preferences, preferences) || other.preferences == preferences)&&const DeepCollectionEquality().equals(other._roles, _roles)&&(identical(other.hasProfile, hasProfile) || other.hasProfile == hasProfile));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hashAll([runtimeType,id,userId,fullName,email,createdAt,phoneNumber,dateOfBirth,gender,bio,profileImageUrl,coverImageUrl,height,weight,preferredSport,experienceLevel,status,isEmailVerified,updatedAt,profileCompletionPercentage,const DeepCollectionEquality().hash(_addresses),contactInformation,preferences,const DeepCollectionEquality().hash(_roles),hasProfile]);

@override
String toString() {
  return 'UserProfileDto(id: $id, userId: $userId, fullName: $fullName, email: $email, createdAt: $createdAt, phoneNumber: $phoneNumber, dateOfBirth: $dateOfBirth, gender: $gender, bio: $bio, profileImageUrl: $profileImageUrl, coverImageUrl: $coverImageUrl, height: $height, weight: $weight, preferredSport: $preferredSport, experienceLevel: $experienceLevel, status: $status, isEmailVerified: $isEmailVerified, updatedAt: $updatedAt, profileCompletionPercentage: $profileCompletionPercentage, addresses: $addresses, contactInformation: $contactInformation, preferences: $preferences, roles: $roles, hasProfile: $hasProfile)';
}


}

/// @nodoc
abstract mixin class _$UserProfileDtoCopyWith<$Res> implements $UserProfileDtoCopyWith<$Res> {
  factory _$UserProfileDtoCopyWith(_UserProfileDto value, $Res Function(_UserProfileDto) _then) = __$UserProfileDtoCopyWithImpl;
@override @useResult
$Res call({
 String id, String userId, String fullName, String email, String createdAt, String? phoneNumber, String? dateOfBirth,@JsonKey(unknownEnumValue: GenderDto.preferNotToSay) GenderDto gender, String? bio, String? profileImageUrl, String? coverImageUrl, String? height, String? weight, String? preferredSport, String? experienceLevel,@JsonKey(unknownEnumValue: UserStatusDto.active) UserStatusDto status, bool isEmailVerified, String? updatedAt, int profileCompletionPercentage, List<AddressDto> addresses, ContactDto? contactInformation, UserPreferenceDto? preferences, List<String> roles, bool hasProfile
});


@override $ContactDtoCopyWith<$Res>? get contactInformation;@override $UserPreferenceDtoCopyWith<$Res>? get preferences;

}
/// @nodoc
class __$UserProfileDtoCopyWithImpl<$Res>
    implements _$UserProfileDtoCopyWith<$Res> {
  __$UserProfileDtoCopyWithImpl(this._self, this._then);

  final _UserProfileDto _self;
  final $Res Function(_UserProfileDto) _then;

/// Create a copy of UserProfileDto
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = null,Object? userId = null,Object? fullName = null,Object? email = null,Object? createdAt = null,Object? phoneNumber = freezed,Object? dateOfBirth = freezed,Object? gender = null,Object? bio = freezed,Object? profileImageUrl = freezed,Object? coverImageUrl = freezed,Object? height = freezed,Object? weight = freezed,Object? preferredSport = freezed,Object? experienceLevel = freezed,Object? status = null,Object? isEmailVerified = null,Object? updatedAt = freezed,Object? profileCompletionPercentage = null,Object? addresses = null,Object? contactInformation = freezed,Object? preferences = freezed,Object? roles = null,Object? hasProfile = null,}) {
  return _then(_UserProfileDto(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,userId: null == userId ? _self.userId : userId // ignore: cast_nullable_to_non_nullable
as String,fullName: null == fullName ? _self.fullName : fullName // ignore: cast_nullable_to_non_nullable
as String,email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,createdAt: null == createdAt ? _self.createdAt : createdAt // ignore: cast_nullable_to_non_nullable
as String,phoneNumber: freezed == phoneNumber ? _self.phoneNumber : phoneNumber // ignore: cast_nullable_to_non_nullable
as String?,dateOfBirth: freezed == dateOfBirth ? _self.dateOfBirth : dateOfBirth // ignore: cast_nullable_to_non_nullable
as String?,gender: null == gender ? _self.gender : gender // ignore: cast_nullable_to_non_nullable
as GenderDto,bio: freezed == bio ? _self.bio : bio // ignore: cast_nullable_to_non_nullable
as String?,profileImageUrl: freezed == profileImageUrl ? _self.profileImageUrl : profileImageUrl // ignore: cast_nullable_to_non_nullable
as String?,coverImageUrl: freezed == coverImageUrl ? _self.coverImageUrl : coverImageUrl // ignore: cast_nullable_to_non_nullable
as String?,height: freezed == height ? _self.height : height // ignore: cast_nullable_to_non_nullable
as String?,weight: freezed == weight ? _self.weight : weight // ignore: cast_nullable_to_non_nullable
as String?,preferredSport: freezed == preferredSport ? _self.preferredSport : preferredSport // ignore: cast_nullable_to_non_nullable
as String?,experienceLevel: freezed == experienceLevel ? _self.experienceLevel : experienceLevel // ignore: cast_nullable_to_non_nullable
as String?,status: null == status ? _self.status : status // ignore: cast_nullable_to_non_nullable
as UserStatusDto,isEmailVerified: null == isEmailVerified ? _self.isEmailVerified : isEmailVerified // ignore: cast_nullable_to_non_nullable
as bool,updatedAt: freezed == updatedAt ? _self.updatedAt : updatedAt // ignore: cast_nullable_to_non_nullable
as String?,profileCompletionPercentage: null == profileCompletionPercentage ? _self.profileCompletionPercentage : profileCompletionPercentage // ignore: cast_nullable_to_non_nullable
as int,addresses: null == addresses ? _self._addresses : addresses // ignore: cast_nullable_to_non_nullable
as List<AddressDto>,contactInformation: freezed == contactInformation ? _self.contactInformation : contactInformation // ignore: cast_nullable_to_non_nullable
as ContactDto?,preferences: freezed == preferences ? _self.preferences : preferences // ignore: cast_nullable_to_non_nullable
as UserPreferenceDto?,roles: null == roles ? _self._roles : roles // ignore: cast_nullable_to_non_nullable
as List<String>,hasProfile: null == hasProfile ? _self.hasProfile : hasProfile // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}

/// Create a copy of UserProfileDto
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$ContactDtoCopyWith<$Res>? get contactInformation {
    if (_self.contactInformation == null) {
    return null;
  }

  return $ContactDtoCopyWith<$Res>(_self.contactInformation!, (value) {
    return _then(_self.copyWith(contactInformation: value));
  });
}/// Create a copy of UserProfileDto
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$UserPreferenceDtoCopyWith<$Res>? get preferences {
    if (_self.preferences == null) {
    return null;
  }

  return $UserPreferenceDtoCopyWith<$Res>(_self.preferences!, (value) {
    return _then(_self.copyWith(preferences: value));
  });
}
}

// dart format on
