# PRD - Academy Module

Version: 1.0

## 1. Purpose
Manage sports academies, infrastructure, coaches, athletes, batches, memberships, and operations.

## 2. Actors
- Academy Admin
- Coach
- Athlete
- Parent
- Platform Admin

## 3. Functional Requirements

### FR-ACD-001 Academy Profile
- Name
- Logo
- Address
- Sports Offered
- Facilities
- Contact Details

### FR-ACD-002 Batch Management
- Create batches
- Assign coaches
- Capacity management
- Batch schedules

### FR-ACD-003 Athlete Enrollment
- Online registration
- Batch assignment
- Attendance
- Progress tracking

### FR-ACD-004 Membership
- Membership plans
- Renewals
- Discounts

### FR-ACD-005 Facility Management
- Grounds
- Courts
- Equipment
- Booking slots

### FR-ACD-006 Reports
- Revenue
- Attendance
- Coach utilization
- Athlete growth

## 4. Business Rules
- Athletes must belong to an active batch.
- Capacity limits are enforced.
- Only verified academies appear in search.

## 5. Database
- Academies
- AcademyBranches
- Batches
- BatchSchedules
- MembershipPlans
- Facilities
- Enrollments

## 6. APIs
GET /api/academies
POST /api/academies
PUT /api/academies/{id}
POST /api/academies/{id}/batches
POST /api/academies/{id}/memberships
GET /api/academies/{id}/reports

## 7. Notifications
- New enrollment
- Membership expiry
- Batch updates

## 8. Security
- Multi-tenant access
- Academy admin permissions
- Audit logs

## 9. Acceptance Criteria
- Academy onboarding
- Batch management
- Athlete enrollment
- Membership lifecycle

## 10. Future
- ERP integrations
- Inventory management
- Smart attendance
- AI utilization reports
