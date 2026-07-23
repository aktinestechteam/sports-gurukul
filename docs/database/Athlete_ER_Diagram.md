# Athlete Domain ER Diagram

> Generated: 2026-07-23  
> Migration: `AddAthleteDomain`  
> Tables: 9 new + 1 existing reference (Users)

## Entity-Relationship Diagram

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           SPORT CATEGORY                                         │
├─────────────────────────────────────────────────────────────────────────────────┤
│ PK  Id                  UUID          NOT NULL                                   │
│     Name                VARCHAR(100)  NOT NULL  UNIQUE                           │
│     Description         VARCHAR(500)  NULL                                        │
│     CreatedAt           TIMESTAMP     NOT NULL                                    │
│     UpdatedAt           TIMESTAMP     NULL                                        │
│     IsDeleted           BOOLEAN       NOT NULL  DEFAULT FALSE                    │
└─────────────────────────────────────────────────────────────────────────────────┘
                                     │
                                     │ 1
                                     │
                                     ▼ *
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              SPORT                                               │
├─────────────────────────────────────────────────────────────────────────────────┤
│ PK  Id                  UUID          NOT NULL                                   │
│ FK  SportCategoryId     UUID          NOT NULL  → SportCategories(Id) RESTRICT  │
│     Name                VARCHAR(100)  NOT NULL  UNIQUE                           │
│     Code                VARCHAR(20)   NOT NULL  UNIQUE                           │
│     OlympicSport        BOOLEAN       NOT NULL  DEFAULT FALSE                    │
│     Description         VARCHAR(500)  NULL                                        │
│     RowVersion          BYTEA         NOT NULL  (Optimistic Concurrency)         │
│     CreatedAt           TIMESTAMP     NOT NULL                                    │
│     UpdatedAt           TIMESTAMP     NULL                                        │
│     IsDeleted           BOOLEAN       NOT NULL  DEFAULT FALSE                    │
└─────────────────────────────────────────────────────────────────────────────────┘
                                     │
                                     │ 1
                                     │
                                     ▼ *
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           ATHLETE                                                │
├─────────────────────────────────────────────────────────────────────────────────┤
│ PK  Id                  UUID          NOT NULL                                   │
│ FK  UserId              UUID          NOT NULL  → Users(Id) CASCADE  UNIQUE      │
│     AthleteCode         VARCHAR(50)   NOT NULL  UNIQUE                           │
│     RegistrationDate    TIMESTAMP     NOT NULL                                    │
│     CurrentLevel        VARCHAR(30)   NOT NULL  DEFAULT 'Beginner'               │
│     ExperienceYears     INTEGER       NOT NULL  DEFAULT 0                        │
│     Height              VARCHAR(20)   NULL                                        │
│     Weight              VARCHAR(20)   NULL                                        │
│     BloodGroup          VARCHAR(20)   NULL                                        │
│     DominantHand        VARCHAR(20)   NULL                                        │
│     DominantFoot        VARCHAR(20)   NULL                                        │
│     Biography           VARCHAR(2000) NULL                                        │
│     Status              VARCHAR(30)   NOT NULL  DEFAULT 'Active'                 │
│     RowVersion          BYTEA         NOT NULL  (Optimistic Concurrency)         │
│     CreatedAt           TIMESTAMP     NOT NULL                                    │
│     UpdatedAt           TIMESTAMP     NULL                                        │
│     IsDeleted           BOOLEAN       NOT NULL  DEFAULT FALSE                    │
└─────────────────────────────────────────────────────────────────────────────────┘
          │                │                │                │
          │ *              │ 1              │ 1              │ 1
          │                │                │                │
          ▼ 1              ▼ 1              ▼ 1              ▼ 1
┌──────────────────┐ ┌──────────────────┐ ┌──────────────────┐ ┌──────────────────┐
│  RANKING         │ │ MEDICAL PROFILE  │ │EMERGENCY CONTACT │ │                  │
├──────────────────┤ ├──────────────────┤ ├──────────────────┤ │                  │
│PK Id    UUID     │ │PK Id      UUID   │ │PK Id      UUID   │ │                  │
│FK AthleteId UUID │ │FK AthleteId UUID │ │FK AthleteId UUID │ │                  │
│  AthleteId UNIQUE│ │  AthleteId UNIQUE│ │  AthleteId UNIQUE│ │                  │
│  CurrentRank     │ │  MedicalConds    │ │  Name  VARCHAR200│ │                  │
│  StateRank       │ │  Allergies       │ │  Relationship    │ │                  │
│  NationalRank    │ │  Medications     │ │  Phone  VARCHAR50│ │                  │
│  IntlRank        │ │  BloodGroup      │ │  Email  VARCHAR200│                 │
│  RankingAuthority│ │  InsuranceNumber │ │  RowVersion      │ │                  │
│  RowVersion      │ │  DoctorName      │ │                  │ │                  │
│  → Athlete CASCADE│ │  DoctorContact   │ │                  │ │                  │
│                  │ │  RowVersion      │ │                  │ │                  │
│                  │ │  → Athlete CASCADE│ │  → Athlete CASCADE│                  │
└──────────────────┘ └──────────────────┘ └──────────────────┘ │                  │
                                                                │                  │
          ┌─────────────────────────────────────────────────────┘                  │
          │                                                                        │
          │ *                                        │ *
          │                                          │
          ▼ *                                        ▼ *
┌──────────────────────────────────┐     ┌──────────────────────────────────┐
│       ATHLETE SPORT              │     │     ATHLETE ACHIEVEMENT          │
├──────────────────────────────────┤     ├──────────────────────────────────┤
│ PK  Id                 UUID      │     │ PK  Id                 UUID      │
│ FK  AthleteId          UUID      │     │ FK  AthleteId          UUID      │
│ FK  SportId            UUID      │     │ FK  AchievementId      UUID      │
│     AthleteId+SportId  UNIQUE    │     │     AthleteId+AchId   UNIQUE    │
│     IsPrimarySport     BOOLEAN   │     │     AwardedDate        TIMESTAMP │
│     JoinedDate         TIMESTAMP │     │     Notes               VARCHAR500│
│     → Athlete CASCADE            │     │     → Athlete CASCADE            │
│     → Sport RESTRICT             │     │     → Achievement CASCADE        │
└──────────────────────────────────┘     └──────────────────────────────────┘
          │                                          │
          │ *                                        │ *
          ▼ 1                                        ▼ 1
┌──────────────────────────────────┐     ┌──────────────────────────────────┐
│         SPORT (above)            │     │        ACHIEVEMENT               │
├──────────────────────────────────┤     ├──────────────────────────────────┤
│ PK  Id                 UUID      │     │ PK  Id                 UUID      │
│     ...                         │     │ Title               VARCHAR200   │
└──────────────────────────────────┘     │ Competition          VARCHAR200  │
                                         │ Position             VARCHAR100  │
                                         │ Level                VARCHAR30   │
                                         │ Date                 TIMESTAMP   │
                                         │ CertificateUrl       VARCHAR2000 │
                                         │ CreatedAt            TIMESTAMP   │
                                         │ UpdatedAt            TIMESTAMP   │
                                         │ IsDeleted            BOOLEAN     │
                                         └──────────────────────────────────┘
```

## Relationship Summary

| Parent Table       | Child Table          | FK Column        | Cardinality     | On Delete  |
|--------------------|----------------------|------------------|-----------------|------------|
| Users              | Athletes             | UserId           | 1:1 (required)  | Cascade    |
| SportCategories    | Sports               | SportCategoryId  | 1:N (required)  | Restrict   |
| Athletes           | MedicalProfiles      | AthleteId        | 1:1 (optional)  | Cascade    |
| Athletes           | EmergencyContacts    | AthleteId        | 1:1 (optional)  | Cascade    |
| Athletes           | Rankings             | AthleteId        | 1:1 (optional)  | Cascade    |
| Athletes           | AthleteSports        | AthleteId        | 1:N (required)  | Cascade    |
| Sports             | AthleteSports        | SportId          | 1:N (required)  | Restrict   |
| Athletes           | AthleteAchievements  | AthleteId        | 1:N (required)  | Cascade    |
| Achievements       | AthleteAchievements  | AchievementId    | 1:N (required)  | Cascade    |

## Index Summary

| Table              | Index Name                                      | Columns                    | Unique |
|--------------------|------------------------------------------------|----------------------------|--------|
| Athletes           | IX_Athletes_UserId                             | UserId                     | Yes    |
| Athletes           | IX_Athletes_AthleteCode                        | AthleteCode                | Yes    |
| Athletes           | IX_Athletes_Status                             | Status                     | No     |
| Sports             | IX_Sports_Name                                 | Name                       | Yes    |
| Sports             | IX_Sports_Code                                 | Code                       | Yes    |
| Sports             | IX_Sports_SportCategoryId                      | SportCategoryId            | No     |
| SportCategories    | IX_SportCategories_Name                        | Name                       | Yes    |
| AthleteSports      | IX_AthleteSports_AthleteId_SportId             | AthleteId, SportId         | Yes    |
| AthleteSports      | IX_AthleteSports_AthleteId                     | AthleteId                  | No     |
| AthleteSports      | IX_AthleteSports_SportId                       | SportId                    | No     |
| AthleteAchievements| IX_AthleteAchievements_AthleteId_AchievementId | AthleteId, AchievementId   | Yes    |
| AthleteAchievements| IX_AthleteAchievements_AthleteId               | AthleteId                  | No     |
| AthleteAchievements| IX_AthleteAchievements_AchievementId           | AchievementId              | No     |
| Rankings           | IX_Rankings_AthleteId                          | AthleteId                  | Yes    |
| MedicalProfiles    | IX_MedicalProfiles_AthleteId                   | AthleteId                  | Yes    |
| EmergencyContacts  | IX_EmergencyContacts_AthleteId                 | AthleteId                  | Yes    |
| Achievements       | IX_Achievements_Title                          | Title                      | No     |
| Achievements       | IX_Achievements_Level                          | Level                      | No     |

## Optimistic Concurrency (RowVersion)

Tables with `IsRowVersion()` for optimistic concurrency handling:
- `Athletes`
- `Sports`
- `Rankings`
- `MedicalProfiles`
- `EmergencyContacts`

## Seed Data

### Sport Categories (5)
| Id     | Name            | Description                        |
|--------|-----------------|------------------------------------|
| b1..01 | Team Sports     | Sports played between two teams    |
| b1..02 | Racquet Sports  | Sports played with racquets        |
| b1..03 | Individual Sports| Individual competitive sports     |
| b1..04 | Combat Sports   | Martial arts and combat disciplines|
| b1..05 | Aquatic Sports  | Water-based sports                 |

### Sports (10)
| Id     | Name          | Code | Olympic | Category       |
|--------|---------------|------|---------|----------------|
| c1..01 | Cricket       | CRK  | No      | Team Sports    |
| c1..02 | Football      | FTB  | Yes     | Team Sports    |
| c1..03 | Badminton     | BDM  | Yes     | Racquet Sports |
| c1..04 | Tennis        | TNS  | Yes     | Racquet Sports |
| c1..05 | Table Tennis  | TTP  | Yes     | Racquet Sports |
| c1..06 | Athletics     | ATH  | Yes     | Individual     |
| c1..07 | Chess         | CHS  | No      | Individual     |
| c1..08 | Swimming      | SWM  | Yes     | Aquatic        |
| c1..09 | Basketball    | BBL  | Yes     | Team Sports    |
| c1..0a | Volleyball    | VLB  | Yes     | Team Sports    |
