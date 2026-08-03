---
title: Sports Gurukul Mobile Design System
version: 1.0
status: Draft
owner: UX Team
---

# 🎨 Sports Gurukul Mobile Design System

> A premium, scalable and consistent design system for the Sports Gurukul Flutter ecosystem.

---

# Table of Contents

1. Design Principles
2. Design Tokens
3. Color System
4. Typography
5. Spacing System
6. Grid System
7. Elevation
8. Border Radius
9. Shadows
10. Icons
11. Illustrations
12. Images
13. Buttons
14. Inputs
15. Cards
16. Lists
17. Chips
18. Badges
19. Navigation
20. Bottom Sheets
21. Dialogs
22. Snackbar
23. Charts
24. Loading States
25. Empty States
26. Error States
27. AI Components
28. Sports Components
29. Motion System
30. Accessibility

---

# 1. Design Philosophy

Sports Gurukul should feel like a premium consumer application rather than enterprise software.

Inspired by

- Apple Fitness
- Nike
- Strava
- Google Material 3
- Linear
- Notion

Core Principles

- Simple
- Beautiful
- Fast
- Consistent
- Accessible
- Delightful

---

# 2. Design Tokens

All UI values must come from centralized tokens.

Never hardcode values.

```dart
AppColors
AppSpacing
AppTypography
AppRadius
AppElevation
AppAnimation
```

---

# 3. Color System

## Primary

| Name        | Hex     | Usage            |
| ----------- | ------- | ---------------- |
| Primary 50  | #E8F2FF | Background       |
| Primary 100 | #CCE4FF | Surface          |
| Primary 200 | #99C9FF | Hover            |
| Primary 300 | #66AEFF | Selected         |
| Primary 400 | #3393FF | Secondary Button |
| Primary 500 | #006DFF | Primary          |
| Primary 600 | #0058CC | Active           |
| Primary 700 | #004399 | Pressed          |
| Primary 800 | #003066 | Dark             |
| Primary 900 | #001C33 | Darkest          |

---

## Success

Green

---

## Warning

Orange

---

## Error

Red

---

## Information

Blue

---

## Neutral

Grey 50

Grey100

Grey200

...

Grey900

---

# Dark Theme

Use Material 3 dark color scheme.

Never invert colors manually.

---

# 4 Typography

## Display

48

40

36

---

## Headings

32

28

24

20

---

## Body

18

16

14

12

---

## Font

Primary

Inter

Fallback

Roboto

---

# Font Weight

Regular

Medium

SemiBold

Bold

---

# 5 Spacing

Use 8-point system.

```
4

8

12

16

20

24

32

40

48

64
```

Never use arbitrary spacing.

---

# 6 Grid

Mobile

4 Columns

Tablet

8 Columns

Desktop

12 Columns

---

# 7 Border Radius

Small

8

Medium

12

Large

20

XL

28

Pill

999

---

# 8 Elevation

Level 0

Flat

Level1

Cards

Level2

Floating Cards

Level3

Dialogs

Level4

Bottom Sheets

---

# 9 Shadows

Use subtle shadows.

Avoid heavy shadows.

```dart
AppShadow.small

AppShadow.medium

AppShadow.large
```

---

# 10 Icons

Use

Material Symbols Rounded

Size

16

20

24

28

32

48

---

# 11 Illustrations

Use modern flat illustrations.

Lottie supported.

SVG preferred.

---

# 12 Images

Use

WebP

Lazy Loading

Progressive Loading

Hero Animations

---

# 13 Buttons

Primary Button

Filled

Secondary Button

Outlined

Ghost Button

Text Button

Danger Button

Icon Button

Floating Button

Loading Button

---

## Button States

Enabled

Disabled

Pressed

Focused

Loading

Success

Error

---

## Minimum Height

56dp

---

# 14 Input Components

Text Field

Password

Phone

OTP

Search

Dropdown

Autocomplete

Date Picker

Time Picker

Slider

Switch

Checkbox

Radio

Segmented Control

---

Validation

Inline

Real Time

Accessible

---

# 15 Cards

Standard Card

Analytics Card

Performance Card

Training Card

Payment Card

Tournament Card

Achievement Card

Coach Card

AI Insight Card

---

Card Padding

16dp

---

# 16 Lists

Simple List

Grouped List

Expandable List

Swipe Actions

Infinite Scroll

---

# 17 Chips

Choice Chip

Filter Chip

Status Chip

Sports Chip

---

# 18 Badges

Primary

Success

Warning

Error

Notification Count

---

# 19 Navigation

Bottom Navigation

Top Tabs

Side Drawer

Navigation Rail

Deep Links

---

Bottom Navigation

Home

Training

AI

Notifications

Profile

---

# 20 Bottom Sheet

Half Sheet

Full Sheet

Modal Sheet

Draggable Sheet

---

# 21 Dialogs

Confirmation

Delete

Error

Warning

Information

Success

---

# 22 Snackbar

Success

Error

Warning

Info

Undo

---

# 23 Charts

Line Chart

Bar Chart

Radar Chart

Progress Ring

Heat Map

Calendar View

Performance Timeline

---

# 24 Loading States

Skeleton Loading

Shimmer

Circular Loader

Linear Loader

Page Loader

---

# 25 Empty States

No Training

No Attendance

No Tournament

No Payments

No Notifications

No Internet

---

Each empty state must contain

Illustration

Title

Description

CTA Button

---

# 26 Error States

Network Error

Server Error

Timeout

Unauthorized

Forbidden

Offline

Retry

Support

---

# 27 AI Components

AI Chat Bubble

AI Coach Card

AI Suggestion Chip

Prompt Input

Voice Input

Streaming Response

Typing Animation

AI Citation Card

AI Feedback Widget

---

# 28 Sports Components

Training Timeline

Attendance Calendar

Performance Radar

Skill Progress

Coach Feedback Card

Workout Card

Tournament Bracket

Match Timeline

Leaderboard

Medal Widget

Achievement Card

Fitness Score

Nutrition Card

---

# 29 Motion System

Page Transition

250ms

Card Animation

200ms

Hero Animation

350ms

Button Press

100ms

Bottom Sheet

300ms

Spring Animation

Default

---

# 30 Accessibility

Touch Target

Minimum 48x48

Contrast

WCAG AA

Dynamic Font

Supported

Screen Reader

Supported

Keyboard Navigation

Supported

Reduce Motion

Supported

---

# Flutter Component Mapping

| Component    | Flutter Widget       |
| ------------ | -------------------- |
| Button       | FilledButton         |
| Card         | Card                 |
| Chip         | FilterChip           |
| Dialog       | AlertDialog          |
| Bottom Sheet | showModalBottomSheet |
| Snackbar     | SnackBar             |
| List         | ListView             |
| Grid         | GridView             |
| Chart        | fl_chart             |

---

# Naming Convention

Buttons

```
PrimaryButton

SecondaryButton

DangerButton
```

Cards

```
TrainingCard

PaymentCard

AttendanceCard

CoachCard
```

Inputs

```
EmailField

PhoneField

PasswordField

SearchField
```

---

# Folder Structure

```
widgets/

buttons/

cards/

dialogs/

charts/

forms/

navigation/

animations/

ai/

sports/

shared/
```

---

# Acceptance Criteria

✔ Consistent UI

✔ Material 3

✔ Dark Theme

✔ Light Theme

✔ Responsive

✔ Accessible

✔ Token Based

✔ Reusable

✔ Testable

---

**End of Design System**
