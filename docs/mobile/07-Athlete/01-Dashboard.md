---
title: Athlete Dashboard
module: Athlete
screen: Dashboard
platform: Flutter
backend: Dashboard Platform
version: 1.0
status: Draft
owner: Product Team
---

# 🏠 Athlete Dashboard

> The Athlete Dashboard is the primary landing page after authentication. It provides a personalized, AI-powered overview of training, attendance, performance, tournaments, finance, notifications, and daily goals.

---

# Table of Contents

1. Purpose
2. Business Goals
3. User Journey
4. Dashboard Philosophy
5. Layout Structure
6. Widget Catalog
7. API Integration
8. State Management
9. Offline Behaviour
10. Analytics
11. Acceptance Criteria

---

# 1. Purpose

The dashboard should answer these questions immediately:

- What should I do today?
- Do I have training?
- Do I have any pending payments?
- Is there a tournament this week?
- How am I performing?
- Has my coach sent feedback?
- What does AI recommend?

Everything important should be visible without navigating to another screen.

---

# 2. Business Goals

- Daily athlete engagement >80%
- Dashboard load <2 seconds
- Personalized experience
- Encourage daily training
- Increase tournament participation
- Improve payment completion
- Surface AI recommendations

---

# 3. User Journey

```text
Login

↓

Dashboard

↓

Choose Activity

↓

Training

↓

Attendance

↓

Performance

↓

AI Coach

↓

Logout
```

---

# 4. Layout Overview

```
┌─────────────────────────────────────┐

☀ Good Morning, Rahul 👋

Profile | Notifications

──────────────────────────────────────

🔥 Today's Goal

"Complete 90 minutes of practice"

[ Start Training ]

──────────────────────────────────────

📅 Today's Schedule

08:00 Fitness

10:00 Cricket Nets

16:00 Strength Training

──────────────────────────────────────

📊 Performance Summary

Fitness Score

Attendance %

Skill Rating

Coach Rating

──────────────────────────────────────

🤖 AI Coach

"You've improved your batting by 8%."

[ View Recommendations ]

──────────────────────────────────────

🏆 Upcoming Tournament

U-19 District Championship

3 Days Left

──────────────────────────────────────

💳 Pending Fees

₹2,500 Due

[ Pay Now ]

──────────────────────────────────────

🏅 Latest Achievement

Bronze Medal

Inter Academy Tournament

──────────────────────────────────────

📢 Announcements

──────────────────────────────────────

⚡ Quick Actions

Check In

Training

Attendance

Performance

Tournament

Payments

Chat

└─────────────────────────────────────┘
```

---

# 5. Widget Priority

## Level 1 (Always Visible)

- Greeting
- Notifications
- Today's Goal
- Today's Training
- Quick Actions

---

## Level 2

- AI Coach
- Performance
- Attendance

---

## Level 3

- Tournament
- Events
- Payments

---

## Level 4

- Announcements
- Coach Message
- Achievements

---

# 6. Dashboard Widgets

## Greeting Card

Shows

- Athlete Name
- Academy
- Sport
- Profile Picture

API

```
GET /api/v1/profile
```

---

## Today's Goal

Shows

- Goal
- Completion %
- Progress Ring

API

```
GET /api/v1/performance/goals
```

---

## Today's Training

Shows

- Time
- Coach
- Venue
- Duration
- Status

API

```
GET /api/v1/training/today
```

CTA

```
Start Training
```

---

## Attendance Widget

Shows

- Today's Status
- Monthly %
- Consecutive Days
- Check-in Button

API

```
GET /api/v1/attendance
```

Action

```
POST /api/v1/attendance/check-in
```

---

## Performance Widget

Displays

- Fitness Score
- Skill Score
- Coach Rating
- Weekly Progress

API

```
GET /api/v1/performance
```

---

## AI Coach Widget

Displays

- Daily Recommendation
- Training Suggestion
- Nutrition Tip
- Recovery Advice

API

```
POST /api/v1/ai/chat
```

Action

```
Open AI Coach
```

---

## Tournament Widget

Displays

- Upcoming Tournament
- Countdown
- Registration Status

API

```
GET /api/v1/tournaments/upcoming
```

---

## Finance Widget

Displays

- Pending Fees
- Due Date
- Last Payment

API

```
GET /api/v1/finance/dashboard
```

---

## Achievement Widget

Displays

- Latest Medal
- Ranking
- Badge
- Milestone

API

```
GET /api/v1/achievements
```

---

## Announcement Widget

Displays

- Academy News
- Training Updates
- Weather Alerts
- Emergency Notices

API

```
GET /api/v1/notifications/dashboard
```

---

# 7. Quick Actions

```
✓ Check In

✓ Training

✓ AI Coach

✓ Performance

✓ Payments

✓ Tournament

✓ Chat

✓ Events
```

---

# 8. Flutter Widget Tree

```text
Scaffold

CustomScrollView

SliverAppBar

GreetingCard

GoalCard

TrainingCard

AttendanceCard

PerformanceCard

AICoachCard

TournamentCard

FinanceCard

AchievementCard

AnnouncementCard

QuickActionGrid

BottomNavigationBar
```

---

# 9. Riverpod Providers

```text
DashboardProvider

ProfileProvider

TrainingProvider

AttendanceProvider

PerformanceProvider

TournamentProvider

FinanceProvider

NotificationProvider

AIProvider
```

Each widget loads independently.

---

# 10. Loading Strategy

Every widget loads asynchronously.

Example

```
Dashboard

↓

Greeting ✓

↓

Training ✓

↓

Performance Loading

↓

AI Loading

↓

Payments ✓
```

One failed widget must never block others.

---

# 11. Empty States

Training

"No training scheduled today."

Performance

"Complete your first training session."

Tournament

"No tournaments available."

Finance

"No pending payments."

AI

"Ask your AI Coach anything."

---

# 12. Offline Behaviour

Available Offline

- Profile
- Cached Dashboard
- Training Schedule
- Attendance History
- Performance Summary
- Previous AI Conversations

Not Available

- Payments
- Live Scores
- AI Streaming
- Tournament Registration

---

# 13. Notifications

Real-time updates for

- Coach Messages
- Training Changes
- Attendance
- Payments
- AI Suggestions
- Tournament Updates

---

# 14. Personalization

Dashboard adapts based on:

- Sport
- Age Group
- Academy
- Training Schedule
- Performance Trends
- Coach Preferences
- User Behavior

---

# 15. Analytics Events

```text
dashboard_opened

training_clicked

attendance_checked

performance_viewed

ai_opened

payment_clicked

tournament_clicked

quick_action_used
```

---

# 16. Performance Goals

Initial Render

< 1.5 seconds

Widget Refresh

< 500 ms

Scroll FPS

60+

Memory Usage

< 120 MB

---

# 17. Accessibility

Supports

- Screen Reader
- Large Fonts
- High Contrast
- Voice Navigation (Future)

---

# 18. Acceptance Criteria

- Dashboard loads in under 2 seconds
- Widgets refresh independently
- Fully responsive
- Offline cache supported
- AI recommendations displayed
- Notification badges update in real time
- Secure API integration
- Accessible and localized
- Supports dark and light themes

---

# Related Backend APIs

| API                                 | Purpose                 |
| ----------------------------------- | ----------------------- |
| GET /api/v1/dashboard               | Dashboard summary       |
| GET /api/v1/profile                 | Athlete profile         |
| GET /api/v1/training/today          | Today's training        |
| GET /api/v1/attendance              | Attendance summary      |
| GET /api/v1/performance             | Performance summary     |
| GET /api/v1/tournaments/upcoming    | Upcoming tournaments    |
| GET /api/v1/finance/dashboard       | Finance summary         |
| GET /api/v1/notifications/dashboard | Dashboard notifications |
| POST /api/v1/ai/chat                | AI Coach                |

---

# Next Screen

- 02-Training.md
- 03-Attendance.md
- 04-Performance.md
- 05-AI-Coach.md
- 06-Tournaments.md

---

**End of Document**
