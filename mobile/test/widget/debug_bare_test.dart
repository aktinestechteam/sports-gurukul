import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  testWidgets('bare TextFormField: tap sets focus', (tester) async {
    final controller = TextEditingController();
    final formKey = GlobalKey<FormState>();
    await tester.pumpWidget(
      MaterialApp(
        home: Scaffold(
          body: Form(
            key: formKey,
            child: TextFormField(
              controller: controller,
              obscureText: true,
              validator: (v) => v == null || v.isEmpty ? 'required' : null,
            ),
          ),
        ),
      ),
    );
    await tester.tap(find.byType(TextFormField));
    await tester.pump();
    await tester.pump();
    debugPrint(
      'BARE FOCUS: ${FocusManager.instance.primaryFocus?.debugLabel ?? 'null'}',
    );

    formKey.currentState!.validate();
    await tester.pump();
    expect(find.text('required'), findsOneWidget);

    await tester.tap(find.byType(TextFormField));
    await tester.pump();
    await tester.pump();
    debugPrint(
      'BARE FOCUS AFTER ERROR: '
      '${FocusManager.instance.primaryFocus?.debugLabel ?? 'null'}',
    );

    await tester.enterText(find.byType(TextFormField), 'hello');
    await tester.pump();
    expect(controller.text, 'hello');
  });
}
