-- =============================================================================
-- Job Service Database (SQL Server)
-- =============================================================================
-- Database       : JobDB
-- Responsibility : Job creation, management, skills, search, and saved jobs
-- SQL Server     : 2019+
-- =============================================================================

IF DB_ID('JobDB') IS NULL
BEGIN
    CREATE DATABASE JobDB;
END
GO

USE JobDB;
GO

-- =============================================================================
-- Jobs
-- =============================================================================
CREATE TABLE Jobs
(
    Id                  BIGINT IDENTITY(1,1) PRIMARY KEY,

    OrganizationId      BIGINT NOT NULL,      -- Organization Service
    RecruiterId         BIGINT NOT NULL,      -- Identity Service

    Title               NVARCHAR(200) NOT NULL,
    Department          NVARCHAR(100) NULL,

    EmploymentType      NVARCHAR(20) NOT NULL,
    Location            NVARCHAR(200) NULL,

    IsRemote            BIT NOT NULL DEFAULT(0),

    ExperienceMin       INT NULL,
    ExperienceMax       INT NULL,

    SalaryMin           DECIMAL(12,2) NULL,
    SalaryMax           DECIMAL(12,2) NULL,

    Currency            NVARCHAR(3) NULL,

    Description         NVARCHAR(MAX) NULL,

    Status              NVARCHAR(20) NOT NULL,

    PublishedDate       DATETIME2 NULL,
    ClosedDate          DATETIME2 NULL,

    IsActive            BIT NULL,
    CreatedOn           DATETIME2 NULL,
    CreatedBy           NVARCHAR(100) NULL,
    UpdatedOn           DATETIME2 NULL,
    UpdatedBy           NVARCHAR(100) NULL,

    CONSTRAINT CK_Jobs_EmploymentType
        CHECK (EmploymentType IN
        ('FullTime','PartTime','Contract','Internship','Temporary')),

    CONSTRAINT CK_Jobs_Status
        CHECK (Status IN ('Draft','Published','Closed')),

    CONSTRAINT CK_Jobs_ExperienceMin
        CHECK (ExperienceMin IS NULL OR ExperienceMin >= 0),

    CONSTRAINT CK_Jobs_ExperienceMax
        CHECK (ExperienceMax IS NULL OR ExperienceMax >= 0),

    CONSTRAINT CK_Jobs_ExperienceRange
        CHECK
        (
            ExperienceMax IS NULL
            OR ExperienceMin IS NULL
            OR ExperienceMax >= ExperienceMin
        ),

    CONSTRAINT CK_Jobs_SalaryRange
        CHECK
        (
            SalaryMax IS NULL
            OR SalaryMin IS NULL
            OR SalaryMax >= SalaryMin
        )
);
GO

-- =============================================================================
-- Job Skills
-- =============================================================================
CREATE TABLE JobSkills
(
    Id                  BIGINT IDENTITY(1,1) PRIMARY KEY,

    JobId               BIGINT NOT NULL,

    SkillName           NVARCHAR(100) NOT NULL,

    IsRequired          BIT NOT NULL DEFAULT(1),

    IsActive            BIT NULL,
    CreatedOn           DATETIME2 NULL,
    CreatedBy           NVARCHAR(100) NULL,
    UpdatedOn           DATETIME2 NULL,
    UpdatedBy           NVARCHAR(100) NULL,

    CONSTRAINT FK_JobSkills_Jobs
        FOREIGN KEY (JobId)
        REFERENCES Jobs(Id)
        ON DELETE CASCADE,

    CONSTRAINT UQ_JobSkills_Job_Skill
        UNIQUE(JobId, SkillName)
);
GO

-- =============================================================================
-- Saved Jobs
-- =============================================================================
CREATE TABLE SavedJobs
(
    Id                  BIGINT IDENTITY(1,1) PRIMARY KEY,

    CandidateId         BIGINT NOT NULL,      -- Candidate Service
    JobId               BIGINT NOT NULL,

    SavedDate           DATETIME2 NOT NULL,

    IsActive            BIT NULL,
    CreatedOn           DATETIME2 NULL,
    CreatedBy           NVARCHAR(100) NULL,
    UpdatedOn           DATETIME2 NULL,
    UpdatedBy           NVARCHAR(100) NULL,

    CONSTRAINT FK_SavedJobs_Jobs
        FOREIGN KEY (JobId)
        REFERENCES Jobs(Id)
        ON DELETE CASCADE,

    CONSTRAINT UQ_SavedJobs
        UNIQUE(CandidateId, JobId)
);
GO

-- =============================================================================
-- Indexes
-- =============================================================================
CREATE INDEX IX_Jobs_OrganizationId
ON Jobs(OrganizationId);
GO

CREATE INDEX IX_Jobs_RecruiterId
ON Jobs(RecruiterId);
GO

CREATE INDEX IX_Jobs_Status
ON Jobs(Status);
GO

CREATE INDEX IX_Jobs_Department
ON Jobs(Department);
GO

CREATE INDEX IX_Jobs_EmploymentType
ON Jobs(EmploymentType);
GO

CREATE INDEX IX_Jobs_Location
ON Jobs(Location);
GO

CREATE INDEX IX_Jobs_PublishedDate
ON Jobs(PublishedDate);
GO

CREATE INDEX IX_JobSkills_JobId
ON JobSkills(JobId);
GO

CREATE INDEX IX_JobSkills_SkillName
ON JobSkills(SkillName);
GO

CREATE INDEX IX_SavedJobs_CandidateId
ON SavedJobs(CandidateId);
GO

-- =============================================================================
-- Full Text Search (Optional)
-- =============================================================================
-- Enable Full-Text Search feature in SQL Server before running.
--
-- CREATE FULLTEXT CATALOG JobCatalog;
--
-- CREATE FULLTEXT INDEX ON Jobs
-- (
--     Title LANGUAGE 1033,
--     Description LANGUAGE 1033
-- )
-- KEY INDEX PK__Jobs__3214EC07
-- ON JobCatalog;