-- =============================================================================
-- Candidate Service Database (SQL Server)
-- =============================================================================
-- Database    : CandidateDb
-- Responsibility: Candidate profiles and resume management
-- SQL Server  : 2019+
-- =============================================================================

IF DB_ID('CandidateDB') IS NULL
BEGIN
    CREATE DATABASE CandidateDB;
END
GO

USE CandidateDB;
GO

-- =============================================================================
-- Candidates
-- =============================================================================
CREATE TABLE Candidates
(
    Id BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_Candidates PRIMARY KEY,

    UserId BIGINT NOT NULL,

    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Phone NVARCHAR(20) NULL,
    Address NVARCHAR(512) NULL,
    LinkedinProfile NVARCHAR(256) NULL,
    PortfolioUrl NVARCHAR(256) NULL,
    Headline NVARCHAR(200) NULL,
    Summary NVARCHAR(MAX) NULL,

    CreatedOn DATETIME2(7) NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedOn DATETIME2(7) NULL,
    UpdatedBy NVARCHAR(100) NULL,

    IsActive BIT NULL,

    CONSTRAINT UQ_Candidates_User UNIQUE (UserId)
);
GO

-- =============================================================================
-- Candidate Skills
-- =============================================================================
CREATE TABLE CandidateSkills
(
    Id BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_CandidateSkills PRIMARY KEY,

    CandidateId BIGINT NOT NULL,

    Name NVARCHAR(100) NOT NULL,
    ProficiencyLevel NVARCHAR(20) NULL,
    YearsOfExperience DECIMAL(4,1) NULL,

    CreatedOn DATETIME2(7) NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedOn DATETIME2(7) NULL,
    UpdatedBy NVARCHAR(100) NULL,

    IsActive BIT NULL,

    CONSTRAINT FK_CandidateSkills_Candidates
        FOREIGN KEY (CandidateId)
        REFERENCES Candidates(Id)
        ON DELETE CASCADE,

    CONSTRAINT CK_CandidateSkills_ProficiencyLevel
        CHECK (ProficiencyLevel IN ('Beginner','Intermediate','Advanced','Expert')),

    CONSTRAINT UQ_CandidateSkills UNIQUE (CandidateId, Name)
);
GO

-- =============================================================================
-- Candidate Experiences
-- =============================================================================
CREATE TABLE CandidateExperiences
(
    Id BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_CandidateExperiences PRIMARY KEY,

    CandidateId BIGINT NOT NULL,

    Company NVARCHAR(200) NOT NULL,
    Title NVARCHAR(150) NOT NULL,
    Location NVARCHAR(150) NULL,

    StartDate DATE NOT NULL,
    EndDate DATE NULL,

    IsCurrent BIT NOT NULL
        CONSTRAINT DF_CandidateExperiences_IsCurrent DEFAULT (0),

    Description NVARCHAR(MAX) NULL,

    CreatedOn DATETIME2(7) NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedOn DATETIME2(7) NULL,
    UpdatedBy NVARCHAR(100) NULL,

    IsActive BIT NULL,

    CONSTRAINT FK_CandidateExperiences_Candidates
        FOREIGN KEY (CandidateId)
        REFERENCES Candidates(Id)
        ON DELETE CASCADE
);
GO

-- =============================================================================
-- Candidate Educations
-- =============================================================================
CREATE TABLE CandidateEducations
(
    Id BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_CandidateEducations PRIMARY KEY,

    CandidateId BIGINT NOT NULL,

    Institution NVARCHAR(200) NOT NULL,
    Degree NVARCHAR(150) NULL,
    FieldOfStudy NVARCHAR(150) NULL,
    StartDate DATE NULL,
    EndDate DATE NULL,
    Grade NVARCHAR(50) NULL,

    CreatedOn DATETIME2(7) NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedOn DATETIME2(7) NULL,
    UpdatedBy NVARCHAR(100) NULL,

    IsActive BIT NULL,

    CONSTRAINT FK_CandidateEducations_Candidates
        FOREIGN KEY (CandidateId)
        REFERENCES Candidates(Id)
        ON DELETE CASCADE
);
GO

-- =============================================================================
-- Candidate Certifications
-- =============================================================================
CREATE TABLE CandidateCertifications
(
    Id BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_CandidateCertifications PRIMARY KEY,

    CandidateId BIGINT NOT NULL,

    Name NVARCHAR(200) NOT NULL,
    IssuingOrg NVARCHAR(200) NULL,
    IssueDate DATE NULL,
    ExpiryDate DATE NULL,
    CredentialId NVARCHAR(150) NULL,
    CredentialUrl NVARCHAR(256) NULL,

    CreatedOn DATETIME2(7) NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedOn DATETIME2(7) NULL,
    UpdatedBy NVARCHAR(100) NULL,

    IsActive BIT NULL,

    CONSTRAINT FK_CandidateCertifications_Candidates
        FOREIGN KEY (CandidateId)
        REFERENCES Candidates(Id)
        ON DELETE CASCADE
);
GO

-- =============================================================================
-- Resumes
-- =============================================================================
CREATE TABLE Resumes
(
    Id BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_Resumes PRIMARY KEY,

    CandidateId BIGINT NOT NULL,

    FileName NVARCHAR(256) NOT NULL,
    FileUrl NVARCHAR(512) NOT NULL,
    FileFormat NVARCHAR(10) NOT NULL,
    FileSize BIGINT NOT NULL,

    IsDefault BIT NOT NULL
        CONSTRAINT DF_Resumes_IsDefault DEFAULT (0),

    UploadedOn DATETIME2(7) NOT NULL,

    CreatedOn DATETIME2(7) NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedOn DATETIME2(7) NULL,
    UpdatedBy NVARCHAR(100) NULL,

    IsActive BIT NULL,

    CONSTRAINT FK_Resumes_Candidates
        FOREIGN KEY (CandidateId)
        REFERENCES Candidates(Id)
        ON DELETE CASCADE,

    CONSTRAINT CK_Resumes_FileFormat
        CHECK (FileFormat IN ('PDF', 'DOCX')),

    CONSTRAINT CK_Resumes_FileSize
        CHECK (FileSize > 0 AND FileSize <= 10485760)
);
GO

-- =============================================================================
-- Indexes
-- =============================================================================

CREATE INDEX IX_Candidates_UserId
ON Candidates(UserId);
GO

CREATE INDEX IX_Candidates_Email
ON Candidates(Email);
GO

CREATE INDEX IX_CandidateSkills_CandidateId
ON CandidateSkills(CandidateId);
GO

CREATE INDEX IX_CandidateExperiences_CandidateId
ON CandidateExperiences(CandidateId);
GO

CREATE INDEX IX_CandidateEducations_CandidateId
ON CandidateEducations(CandidateId);
GO

CREATE INDEX IX_CandidateCertifications_CandidateId
ON CandidateCertifications(CandidateId);
GO

CREATE INDEX IX_Resumes_CandidateId
ON Resumes(CandidateId);
GO

-- Only one default resume per candidate
SET QUOTED_IDENTIFIER ON;
GO

CREATE UNIQUE INDEX UX_Resumes_DefaultPerCandidate
ON Resumes(CandidateId)
WHERE IsDefault = 1;
GO