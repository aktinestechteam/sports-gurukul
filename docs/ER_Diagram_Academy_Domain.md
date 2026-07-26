# Academy Domain - ER Diagram

```mermaid
erDiagram
    Academy {
        uuid Id PK
        string AcademyCode UK
        string Name
        string LegalName
        string Description
        string RegistrationNumber UK
        string GSTNumber
        datetime EstablishedDate
        string Website
        string Email UK
        string Phone
        string Status
        string VerificationStatus
        string LogoUrl
        string BannerUrl
        bytea RowVersion
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
    }

    AcademyBranch {
        uuid Id PK
        uuid AcademyId FK
        string BranchName
        string Address
        string Country
        string State
        string City
        string District
        string PostalCode
        decimal Latitude
        decimal Longitude
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
    }

    AcademySport {
        uuid Id PK
        uuid AcademyId FK
        uuid SportId FK
        bool IsPrimarySport
        datetime JoinedDate
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
    }

    AcademyFacility {
        uuid Id PK
        uuid AcademyId FK
        string FacilityName
        string FacilityType
        string IndoorOutdoor
        int Capacity
        bool Available
        string Description
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
    }

    AcademyOperatingHours {
        uuid Id PK
        uuid AcademyId FK
        time MondayOpening
        time MondayClosing
        time TuesdayOpening
        time TuesdayClosing
        time WednesdayOpening
        time WednesdayClosing
        time ThursdayOpening
        time ThursdayClosing
        time FridayOpening
        time FridayClosing
        time SaturdayOpening
        time SaturdayClosing
        time SundayOpening
        time SundayClosing
        string HolidaySchedule
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
    }

    AcademyContact {
        uuid Id PK
        uuid AcademyId FK
        string PrimaryContactName
        string PrimaryPhone
        string PrimaryEmail
        string SecondaryContactName
        string SecondaryPhone
        string SecondaryEmail
        string Address
        string Country
        string State
        string City
        string PostalCode
        decimal Latitude
        decimal Longitude
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
    }

    AcademySocialLink {
        uuid Id PK
        uuid AcademyId FK
        string Platform
        string Url
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
    }

    AcademyMembership {
        uuid Id PK
        uuid AcademyId FK
        string MembershipName
        string Description
        decimal Price
        int Duration
        string Benefits
        string Status
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
    }

    AcademyVerification {
        uuid Id PK
        uuid AcademyId FK
        string VerificationStatus
        uuid VerifiedBy
        datetime VerifiedOn
        string Remarks
        bytea RowVersion
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
    }

    AcademyDocument {
        uuid Id PK
        uuid AcademyId FK
        string Category
        string Title
        string Description
        string OriginalFileName
        string StoredFileName
        string StorageProvider
        string StoragePath
        string MimeType
        string Extension
        bigint FileSize
        string Checksum
        int Version
        string Status
        uuid UploadedBy
        datetime UploadedOn
        uuid VerifiedBy
        datetime VerifiedOn
        datetime ExpiryDate
        bool IsPublic
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
    }

    AcademyGallery {
        uuid Id PK
        uuid AcademyId FK
        string Title
        string Description
        string ImageUrl
        string ThumbnailUrl
        int SortOrder
        bool IsFeatured
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
    }

    Sport {
        uuid Id PK
        string Name
        string Code
        bool OlympicSport
        string Description
        uuid SportCategoryId FK
        bytea RowVersion
    }

    Academy ||--o{ AcademyBranch : "has many branches"
    Academy ||--o{ AcademySport : "offers many sports"
    Academy ||--o{ AcademyFacility : "has many facilities"
    Academy ||--o{ AcademyOperatingHours : "has one operating hours"
    Academy ||--o{ AcademyContact : "has one contact"
    Academy ||--o{ AcademySocialLink : "has many social links"
    Academy ||--o{ AcademyMembership : "has many membership plans"
    Academy ||--o{ AcademyVerification : "has one verification"
    Academy ||--o{ AcademyDocument : "has many documents"
    Academy ||--o{ AcademyGallery : "has many gallery images"
    AcademySport }o--|| Sport : "references sport"
```

## Relationship Summary

| Parent | Relationship | Child | FK | Delete Behavior |
|--------|-------------|-------|-----|-----------------|
| Academy | 1 : N | AcademyBranch | AcademyId | Cascade |
| Academy | 1 : N | AcademySport | AcademyId | Cascade |
| Academy | 1 : N | AcademyFacility | AcademyId | Cascade |
| Academy | 1 : 1 | AcademyOperatingHours | AcademyId | Cascade |
| Academy | 1 : 1 | AcademyContact | AcademyId | Cascade |
| Academy | 1 : N | AcademySocialLink | AcademyId | Cascade |
| Academy | 1 : N | AcademyMembership | AcademyId | Cascade |
| Academy | 1 : 1 | AcademyVerification | AcademyId | Cascade |
| Academy | 1 : N | AcademyDocument | AcademyId | Cascade |
| Academy | 1 : N | AcademyGallery | AcademyId | Cascade |
| Sport | 1 : N | AcademySport | SportId | Restrict |

## Unique Constraints

| Table | Columns | Notes |
|-------|---------|-------|
| Academies | AcademyCode | Unique academy code |
| Academies | Email | Unique email |
| Academies | RegistrationNumber | Unique (nullable filter) |
| AcademyBranches | AcademyId + BranchName | Unique branch per academy |
| AcademySports | AcademyId + SportId | Unique sport per academy |
| AcademySocialLinks | AcademyId + Platform | One link per platform |
| AcademyMemberships | AcademyId + MembershipName | Unique name per academy |
| AcademyContacts | AcademyId | One contact per academy |
| AcademyOperatingHours | AcademyId | One schedule per academy |
| AcademyVerifications | AcademyId | One verification per academy |

## Indexes

| Table | Index | Type |
|-------|-------|------|
| Academies | AcademyCode | Unique |
| Academies | Email | Unique |
| Academies | RegistrationNumber | Unique (partial) |
| Academies | Name | Non-unique |
| Academies | Phone | Non-unique |
| Academies | Status | Non-unique |
| Academies | VerificationStatus | Non-unique |
| Academies | Status + CreatedAt | Composite |
| AcademyBranches | AcademyId | Non-unique |
| AcademyBranches | AcademyId + BranchName | Unique |
| AcademyBranches | Country | Non-unique |
| AcademyBranches | State + City | Composite |
| AcademySports | AcademyId + SportId | Unique |
| AcademySports | AcademyId | Non-unique |
| AcademySports | SportId | Non-unique |
| AcademyFacilities | AcademyId | Non-unique |
| AcademyFacilities | FacilityType | Non-unique |
| AcademyFacilities | AcademyId + FacilityType | Composite |
| AcademyMemberships | AcademyId | Non-unique |
| AcademyMemberships | AcademyId + MembershipName | Unique |
| AcademyMemberships | Status | Non-unique |
| AcademyDocuments | AcademyId | Non-unique |
| AcademyDocuments | Category | Non-unique |
| AcademyDocuments | Status | Non-unique |
| AcademyDocuments | UploadedOn | Non-unique |
| AcademyDocuments | AcademyId + Category | Composite |
| AcademyDocuments | AcademyId + IsDeleted | Composite |
| AcademyGalleries | AcademyId | Non-unique |
| AcademyGalleries | AcademyId + SortOrder | Composite |
| AcademyGalleries | AcademyId + IsFeatured | Composite |
| AcademySocialLinks | AcademyId | Non-unique |
| AcademySocialLinks | AcademyId + Platform | Unique |
| AcademyOperatingHours | AcademyId | Unique |
| AcademyContacts | AcademyId | Unique |
| AcademyVerifications | AcademyId | Unique |
| AcademyVerifications | VerificationStatus | Non-unique |
