---
title: Welcome & Onboarding Screen
module: Authentication
platform: Flutter
version: 1.0
status: Draft
owner: Product Team
backend_dependency: None
---

# 👋 Welcome Screen

> The Welcome Screen is the user's first experience with Sports Gurukul. It introduces the platform, highlights key features, allows language selection, and guides the user to authentication.

---

# Objectives

The Welcome Screen should:

- Create a premium first impression
- Explain the value of Sports Gurukul
- Allow language selection
- Support dark/light mode
- Allow users to Login or Register
- Support future onboarding updates from Remote Config

---

# User Story

As a first-time user,

I want to understand what Sports Gurukul offers,

so that I feel confident before creating an account.

---

# Target Users

- Athlete
- Parent
- Coach

---

# Screen Layout

```
┌────────────────────────────────────┐

          Sports Gurukul Logo

       Train • Compete • Excel

──────────────────────────────────────

        Illustration / Animation

   "Your Complete Sports Companion"

──────────────────────────────────────

✓ Training Management

✓ Performance Tracking

✓ AI Coach

✓ Tournament Participation

✓ Payments

✓ Notifications

──────────────────────────────────────

🌐 Language

English ▼

──────────────────────────────────────

[ Login ]

[ Create New Account ]

──────────────────────────────────────

Privacy Policy

Terms & Conditions

App Version

└────────────────────────────────────┘
```

---

# Hero Section

Display

- Animated logo
- Academy branding
- AI-inspired sports illustration

Use Lottie animation.

---

# Feature Carousel

Auto-scroll every 4 seconds.

Pages:

### Slide 1

🏋️ Smart Training

Description

Manage training schedules, attendance, and progress.

---

### Slide 2

📊 Performance Analytics

Track performance using AI-driven insights.

---

### Slide 3

🏆 Tournament Management

Register, participate, and monitor tournaments.

---

### Slide 4

🤖 AI Coach

Receive personalized coaching recommendations.

---

### Slide 5

💳 Digital Payments

Pay fees securely and download receipts.

---

# Primary Actions

## Login

Navigate

```
/login
```

---

## Register

Navigate

```
/register
```

---

# Secondary Actions

Language

Theme

Privacy Policy

Terms & Conditions

Contact Support

---

# Navigation Flow

```
Welcome

├── Login

├── Register

├── Privacy Policy

├── Terms

└── Language Selection
```

---

# Flutter Widget Tree

```
Scaffold

SafeArea

Column

Logo

Animated Illustration

CarouselSlider

Feature Cards

Language Selector

Primary Button

Secondary Button

Footer Links
```

---

# State Management

Riverpod Providers

```
LanguageProvider

ThemeProvider

RemoteConfigProvider

ConnectivityProvider
```

---

# API Dependencies

None.

All content should be bundled locally.

Future enhancement:

Remote Config may update

- Banner
- Announcement
- Feature highlights

---

# Offline Behaviour

Works fully offline.

No internet required.

---

# Theme Support

Light

Dark

System Default

---

# Language Support

Initial Languages

- English
- Hindi
- Marathi

Future

- Tamil
- Telugu
- Kannada
- Bengali
- Gujarati

---

# Accessibility

Supports

- Screen Reader
- Dynamic Text Scaling
- High Contrast
- RTL-ready Layout
- Voice Navigation (Future)

---

# Animations

Logo Fade

600ms

Carousel Transition

350ms

Button Ripple

Material 3

Page Transition

250ms

---

# Analytics Events

```
welcome_opened

feature_slide_changed

language_selected

login_clicked

register_clicked

privacy_opened

terms_opened
```

---

# Security

No authentication required.

No sensitive data stored.

---

# Performance Goals

Initial Render

< 500 ms

Animation FPS

60 FPS

Memory Usage

< 40 MB

---

# Acceptance Criteria

- Premium onboarding experience
- Responsive layout
- Smooth carousel
- Multi-language ready
- Dark & light theme support
- Offline capable
- Accessible
- Analytics integrated

---

# Future Enhancements

- Personalized onboarding
- Academy-specific branding
- Video introduction
- AI-powered walkthrough
- Dynamic onboarding via Remote Config

---

# Navigation Targets

| Action             | Route           |
| ------------------ | --------------- |
| Login              | /login          |
| Register           | /register       |
| Privacy Policy     | /privacy-policy |
| Terms & Conditions | /terms          |
| Language           | Bottom Sheet    |

---

# Related Documents

- 01-Splash.md
- 03-Login.md
- 04-OTP-Verification.md

---

**End of Document**
