-- PostgreSQL Enterprise Schema (Foundation)

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE Users (
    Id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    Email VARCHAR(255) UNIQUE NOT NULL,
    PasswordHash TEXT NOT NULL,
    FullName VARCHAR(200) NOT NULL,
    Status VARCHAR(30) NOT NULL,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedOn TIMESTAMP,
    IsDeleted BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE Roles (
    Id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    Name VARCHAR(100) UNIQUE NOT NULL
);

CREATE TABLE UserRoles (
    UserId UUID REFERENCES Users(Id),
    RoleId UUID REFERENCES Roles(Id),
    PRIMARY KEY(UserId, RoleId)
);

CREATE TABLE Athletes (
    Id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    UserId UUID REFERENCES Users(Id),
    PrimarySport VARCHAR(100),
    SkillLevel VARCHAR(50)
);

CREATE TABLE Coaches (
    Id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    UserId UUID REFERENCES Users(Id),
    ExperienceYears INT
);

CREATE TABLE Academies (
    Id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    Name VARCHAR(200) NOT NULL,
    City VARCHAR(100)
);

CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Athletes_Sport ON Athletes(PrimarySport);
