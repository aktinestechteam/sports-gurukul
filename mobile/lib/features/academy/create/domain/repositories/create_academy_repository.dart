import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_contact.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/create_academy_params.dart';

/// Contract for academy creation backed by the backend academies API.
///
/// Implementations in infrastructure talk to the network; domain and
/// application layers depend only on this abstraction. The operation returns
/// a [Result] so failures flow to the UI as data, never as raw exceptions.
abstract interface class CreateAcademyRepository {
  /// Creates a new academy with [params] and returns the created record.
  Future<Result<Academy>> createAcademy(CreateAcademyParams params);

  /// Fetches an existing academy by [academyId].
  Future<Result<Academy>> getAcademy(String academyId);

  /// Resolves the current user's owned academy, or a success holding null
  /// when they own none (e.g. a brand-new account).
  Future<Result<Academy?>> getMyAcademy();

  /// Updates an existing academy's core fields with [params] (name,
  /// description, contact email/phone, website) and uploads any branding
  /// images chosen in [params]. Academy type and sports are not editable
  /// through this endpoint.
  Future<Result<Academy>> updateAcademy(
    String academyId,
    CreateAcademyParams params,
  );

  /// Updates an existing academy's contact + address block with [params].
  Future<Result<AcademyContact>> updateAcademyContact(
    String academyId,
    CreateAcademyParams params,
  );
}
