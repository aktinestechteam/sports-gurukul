import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/validators/email_validator.dart';
import 'package:sports_gurukul/core/validators/otp_validator.dart';
import 'package:sports_gurukul/core/validators/password_validator.dart';
import 'package:sports_gurukul/core/validators/phone_validator.dart';
import 'package:sports_gurukul/core/validators/required_validator.dart';
import 'package:sports_gurukul/core/validators/validation_error.dart';
import 'package:sports_gurukul/core/validators/validator.dart';

void main() {
  group('EmailValidator', () {
    const validator = EmailValidator();

    test('accepts well-formed addresses', () {
      expect(validator.validate('user@example.com'), isNull);
      expect(validator.validate('first.last+tag@sub.example.co.in'), isNull);
      expect(validator.isValid('user@example.com'), isTrue);
    });

    test('rejects malformed addresses', () {
      expect(validator.validate('user@'), isNotNull);
      expect(validator.validate('@example.com'), isNotNull);
      expect(validator.validate('user example.com'), isNotNull);
      expect(validator.validate('user@exa mple.com'), isNotNull);
    });

    test('treats empty input as valid', () {
      expect(validator.validate(null), isNull);
      expect(validator.validate(''), isNull);
      expect(validator.validate('   '), isNull);
    });
  });

  group('PhoneValidator', () {
    const validator = PhoneValidator();

    test('accepts 10-digit numbers starting with 6-9', () {
      expect(validator.validate('9876543210'), isNull);
      expect(validator.validate('6123456789'), isNull);
    });

    test('rejects invalid numbers', () {
      expect(validator.validate('5123456789'), isNotNull);
      expect(validator.validate('12345'), isNotNull);
      expect(validator.validate('987654321'), isNotNull);
      expect(validator.validate('98765432100'), isNotNull);
      expect(validator.validate('98765a3210'), isNotNull);
    });

    test('treats empty input as valid', () {
      expect(validator.validate(null), isNull);
      expect(validator.validate(''), isNull);
    });
  });

  group('OtpValidator', () {
    const validator = OtpValidator();

    test('accepts the default 6-digit OTP', () {
      expect(validator.validate('123456'), isNull);
      expect(validator.isValid('000000'), isTrue);
    });

    test('rejects wrong length and non-numeric input', () {
      expect(validator.validate('12345'), isNotNull);
      expect(validator.validate('1234567'), isNotNull);
      expect(validator.validate('12a456'), isNotNull);
    });

    test('supports a custom length', () {
      const fourDigit = OtpValidator(length: 4);
      expect(fourDigit.validate('1234'), isNull);
      expect(fourDigit.validate('12345'), isNotNull);
    });

    test('treats empty input as valid', () {
      expect(validator.validate(null), isNull);
    });
  });

  group('PasswordValidator', () {
    test('applies the minimum length by default', () {
      const validator = PasswordValidator();
      expect(validator.validate('short'), isNotNull);
      expect(validator.validate('12345678'), isNull);
      final error = validator.validate('123');
      expect(error?.code, 'validation.password.tooShort');
      expect(error?.params, {'min': 8});
    });

    test('enforces configured complexity requirements', () {
      const strict = PasswordValidator(
        requireUppercase: true,
        requireDigit: true,
        requireSpecialCharacter: true,
      );
      expect(strict.validate('abcdefgh'), isNotNull);
      expect(strict.validate('Abcdefgh'), isNotNull);
      expect(strict.validate('Abcdefgh1'), isNotNull);
      expect(strict.validate('Abcdefgh1!'), isNull);
    });

    test('enforces a custom maximum length', () {
      const validator = PasswordValidator(maxLength: 10);
      expect(validator.validate('12345678901'), isNotNull);
      expect(validator.validate('1234567890'), isNull);
    });

    test('treats empty input as valid', () {
      expect(const PasswordValidator().validate(null), isNull);
      expect(const PasswordValidator().validate(''), isNull);
    });
  });

  group('RequiredValidator', () {
    const validator = RequiredValidator<String>();

    test('rejects missing values', () {
      expect(validator.validate(null), isNotNull);
      expect(validator.validate(''), isNotNull);
      expect(validator.validate('   '), isNotNull);
    });

    test('accepts present values', () {
      expect(validator.validate('x'), isNull);
      expect(validator.validate('0'), isNull);
    });

    test('works with collections and numbers', () {
      const requiredList = RequiredValidator<List<int>>();
      const requiredMap = RequiredValidator<Map<String, String>>();
      expect(requiredList.validate(const []), isNotNull);
      expect(requiredList.validate(const [1]), isNull);
      expect(requiredMap.validate(const {}), isNotNull);
      expect(const RequiredValidator<int>().validate(0), isNull);
    });
  });

  group('Validator framework', () {
    test('ValidationError carries a code and optional params', () {
      const error = ValidationError(
        'validation.password.tooShort',
        params: {
          'min': 8,
        },
      );
      expect(error.code, 'validation.password.tooShort');
      expect(error.params, {'min': 8});
    });

    test('ValidationError equality includes params', () {
      const a = ValidationError('x', params: {'k': 1});
      const b = ValidationError('x', params: {'k': 1});
      const c = ValidationError('x');
      expect(a, b);
      expect(a, isNot(c));
    });

    test('CompositeValidator returns the first error and short-circuits', () {
      const validator = CompositeValidator<String>([
        RequiredValidator<String>(),
        EmailValidator(),
      ]);

      expect(validator.validate(null), isNotNull);
      expect(validator.validate('not-an-email'), isNotNull);
      expect(validator.validate('user@example.com'), isNull);
    });

    test('validators pass empty input through to later validators', () {
      const ordered = CompositeValidator<String>([
        EmailValidator(),
        RequiredValidator<String>(),
      ]);
      expect(ordered.validate('')?.code, 'validation.required');
    });
  });
}
