import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/utils/file_utils.dart';
import 'package:sports_gurukul/core/utils/image_utils.dart';

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  group('FileUtils', () {
    test('extracts extensions', () {
      expect(FileUtils.extensionOf('a/b/photo.PNG'), 'png');
      expect(FileUtils.extensionOf('archive.tar.gz'), 'gz');
      expect(FileUtils.extensionOf('noext'), '');
      expect(FileUtils.extensionOf('trailing.'), '');
    });

    test('extracts the file name', () {
      expect(FileUtils.fileNameOf('a/b/file.txt'), 'file.txt');
      expect(FileUtils.fileNameOf(r'a\b\file.txt'), 'file.txt');
      expect(FileUtils.fileNameOf('file.txt'), 'file.txt');
    });

    test('sanitizes invalid file-name characters', () {
      expect(FileUtils.sanitizeFileName('a/b:c*d?'), 'a_b_c_d_');
      expect(FileUtils.sanitizeFileName('report.pdf'), 'report.pdf');
    });

    test('writes temp files and reads them back', () async {
      final bytes = utf8.encode('hello core');
      final file = await FileUtils.writeTempFile('note.txt', bytes);
      try {
        expect(file.existsSync(), isTrue);
        expect(file.readAsStringSync(), 'hello core');
      } finally {
        file.parent.deleteSync(recursive: true);
      }
    });

    test('sizeLabel delegates to the byte formatter', () {
      expect(FileUtils.sizeLabel(1024), '1 KB');
    });
  });

  group('ImageUtils', () {
    test('maps known extensions to MIME types', () {
      expect(ImageUtils.mimeTypeOf('photo.jpg'), 'image/jpeg');
      expect(ImageUtils.mimeTypeOf('photo.JPEG'), 'image/jpeg');
      expect(ImageUtils.mimeTypeOf('photo.png'), 'image/png');
      expect(ImageUtils.mimeTypeOf('photo.webp'), 'image/webp');
      expect(ImageUtils.mimeTypeOf('photo.pdf'), isNull);
    });

    test('detects supported image extensions', () {
      expect(ImageUtils.isSupportedImage('pic.gif'), isTrue);
      expect(ImageUtils.isSupportedImage('pic.pdf'), isFalse);
    });

    test('checks size limits', () {
      expect(ImageUtils.isWithinSizeLimit(100), isTrue);
      expect(ImageUtils.isWithinSizeLimit(5 * 1024 * 1024), isTrue);
      expect(ImageUtils.isWithinSizeLimit(6 * 1024 * 1024), isFalse);
      expect(ImageUtils.isWithinSizeLimit(100, maxBytes: 200), isTrue);
      expect(ImageUtils.isWithinSizeLimit(-1), isFalse);
    });
  });
}
