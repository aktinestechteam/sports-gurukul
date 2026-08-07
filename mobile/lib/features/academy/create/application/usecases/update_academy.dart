import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_contact.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/create_academy_params.dart';
import 'package:sports_gurukul/features/academy/create/domain/repositories/create_academy_repository.dart';

/// Updates an existing academy's core fields (plus any branding images chosen
/// in the wizard) for the given academy id.
class UpdateAcademy {
  const UpdateAcademy(this._repository);

  final CreateAcademyRepository _repository;

  Future<Result<Academy>> call(String academyId, CreateAcademyParams params) =>
      _repository.updateAcademy(academyId, params);
}

/// Updates an existing academy's contact + address block for the given
/// academy id.
class UpdateAcademyContact {
  const UpdateAcademyContact(this._repository);

  final CreateAcademyRepository _repository;

  Future<Result<AcademyContact>> call(
    String academyId,
    CreateAcademyParams params,
  ) => _repository.updateAcademyContact(academyId, params);
}
