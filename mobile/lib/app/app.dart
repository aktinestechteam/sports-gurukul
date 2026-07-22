import 'package:flutter/material.dart';

class SportsGurukulApp extends StatelessWidget {
  const SportsGurukulApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Sports Gurukul',
      theme: ThemeData(
        colorSchemeSeed: Colors.green,
        useMaterial3: true,
      ),
      home: const Scaffold(
        body: Center(
          child: Text('Sports Gurukul'),
        ),
      ),
    );
  }
}
